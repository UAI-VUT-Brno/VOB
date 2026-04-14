// PvZLike_Procedural_WinForms.cs
// .NET 6+ WinForms single-file demo (procedural).
//
//
// Controls:
// - Left click on a cell to place currently selected plant.
// - Keys: 1 = PeaShooter, 2 = Sunflower, 3 = Wallnut, 4 = Bomb
// - Space = toggle fast-forward (2x sim)
// - R = restart
//
// Rules (simplified):
// - Sunflowers generate sun points.
// - PeaShooters shoot peas to the right in their lane.
// - Wallnuts are tanky blockers.
// - Bomb explodes in 1.2s and damages zombies in area.
// - Zombies spawn on the right, move left, attack plants.
// - Lose if any zombie reaches left edge.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PvZLikeDemo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new GameForm());
        }
    }

    // one Form class, everything else is structs/enums + static arrays and functions.
    public sealed class GameForm : Form
    {
        // ----------------------------
        // "Global" state (static-ish)
        // ----------------------------
        private const int Rows = 5;
        private const int Cols = 9;

        private const int CellW = 90;
        private const int CellH = 90;

        private const int LawnLeft = 30;
        private const int LawnTop = 30;

        private const int MaxZombies = 128;
        private const int MaxProjectiles = 256;
        private const int MaxParticles = 256;

        private const float BaseDt = 1f / 60f;

        private readonly System.Windows.Forms.Timer _timer;

        private bool _fastForward = false;
        private float _accum = 0f;
        private long _lastTicks;

        private readonly Random _rng = new Random(12345);

        private int _sun = 50;
        private int _wave = 0;
        private float _time = 0f;
        private bool _lost = false;

        private PlantType _selectedPlant = PlantType.PeaShooter;

        // Board: one plant per cell
        private PlantCell[,] _plants = new PlantCell[Rows, Cols];

        // Entities: zombies & projectiles
        private Zombie[] _zombies = new Zombie[MaxZombies];
        private int _zombieCount = 0;

        private Projectile[] _projs = new Projectile[MaxProjectiles];
        private int _projCount = 0;

        private Particle[] _particles = new Particle[MaxParticles];
        private int _particleCount = 0;

        // Spawn pacing
        private float _spawnTimer = 0f;
        private float _spawnInterval = 2.2f;

        // A simple lane occupancy heuristic used by shooter logic
        private int[] _zombieCountInLane = new int[Rows];

        public GameForm()
        {
            Text = "PvZ-like (Procedural)";
            DoubleBuffered = true;
            ClientSize = new Size(LawnLeft + Cols * CellW + 300, LawnTop + Rows * CellH + 60);
            BackColor = Color.FromArgb(26, 28, 30);
            KeyPreview = true;

            _timer = new System.Windows.Forms.Timer { Interval = 16 };
            _timer.Tick += (_, __) => TickGame();

            MouseDown += OnMouseDown;
            KeyDown += OnKeyDown;

            ResetGame();
            _lastTicks = Stopwatch.GetTimestamp();
            _timer.Start();
        }

        private void ResetGame()
        {
            _sun = 150;
            _wave = 0;
            _time = 0f;
            _lost = false;

            _selectedPlant = PlantType.PeaShooter;

            _plants = new PlantCell[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _plants[r, c] = PlantCell.Empty();

            _zombies = new Zombie[MaxZombies];
            _zombieCount = 0;

            _projs = new Projectile[MaxProjectiles];
            _projCount = 0;

            _particles = new Particle[MaxParticles];
            _particleCount = 0;

            Array.Fill(_zombieCountInLane, 0);

            _spawnTimer = 0f;
            _spawnInterval = 2.2f;
            _accum = 0f;
        }

        // ----------------------------
        // Input
        // ----------------------------
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1) _selectedPlant = PlantType.PeaShooter;
            if (e.KeyCode == Keys.D2) _selectedPlant = PlantType.Sunflower;
            if (e.KeyCode == Keys.D3) _selectedPlant = PlantType.Wallnut;
            if (e.KeyCode == Keys.D4) _selectedPlant = PlantType.Bomb;

            if (e.KeyCode == Keys.Space) _fastForward = !_fastForward;
            if (e.KeyCode == Keys.R) ResetGame();
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (_lost) return;

            var (hit, r, c) = ScreenToCell(e.Location);
            if (!hit) return;

            // Place plant
            if (_plants[r, c].Type != PlantType.None) return;

            int cost = GetPlantCost(_selectedPlant);
            if (_sun < cost) return;

            _sun -= cost;

            _plants[r, c] = PlantCell.Create(_selectedPlant);

            // Small feedback particles
            SpawnParticles(CellCenterX(c), CellCenterY(r), 10, Color.FromArgb(240, 220, 140));
            Invalidate();
        }

        // ----------------------------
        // Main loop (fixed step)
        // ----------------------------
        private void TickGame()
        {
            long now = Stopwatch.GetTimestamp();
            double seconds = (now - _lastTicks) / (double)Stopwatch.Frequency;
            _lastTicks = now;

            float dt = (float)seconds;
            if (_fastForward) dt *= 2f;

            // accumulate and simulate fixed steps
            _accum += dt;
            while (_accum >= BaseDt)
            {
                Step(BaseDt);
                _accum -= BaseDt;
            }

            Invalidate();
        }

        private void Step(float dt)
        {
            if (_lost) return;

            _time += dt;

            // Make waves gradually harder
            if ((int)(_time / 25f) > _wave)
            {
                _wave = (int)(_time / 25f);
                _spawnInterval = Math.Max(0.75f, 2.2f - _wave * 0.18f);
            }

            // Recompute lane counts
            Array.Fill(_zombieCountInLane, 0);
            for (int i = 0; i < _zombieCount; i++)
            {
                if (_zombies[i].Alive)
                    _zombieCountInLane[_zombies[i].Row]++;
            }

            UpdatePlants(dt);
            SpawnZombies(dt);
            UpdateZombies(dt);
            UpdateProjectiles(dt);
            ResolveCombat(dt);
            UpdateParticles(dt);

            // Lose condition
            for (int i = 0; i < _zombieCount; i++)
            {
                if (_zombies[i].Alive && _zombies[i].X < LawnLeft - 10)
                {
                    _lost = true;
                    SpawnParticles(LawnLeft + 10, LawnTop + Rows * CellH / 2, 50, Color.FromArgb(240, 90, 90));
                    break;
                }
            }
        }

        // ----------------------------
        // Plants
        // ----------------------------
        private void UpdatePlants(float dt)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    ref var cell = ref _plants[r, c];
                    if (cell.Type == PlantType.None) continue;

                    cell.Cooldown -= dt;
                    cell.AuxTimer -= dt;

                    switch (cell.Type)
                    {
                        case PlantType.Sunflower:
                            // generate sun periodically
                            if (cell.AuxTimer <= 0f)
                            {
                                cell.AuxTimer = 6.0f;
                                _sun += 25;
                                SpawnParticles(CellCenterX(c), CellCenterY(r), 8, Color.FromArgb(250, 215, 80));
                            }
                            break;

                        case PlantType.PeaShooter:
                            // shoot only if there's any zombie in lane
                            if (_zombieCountInLane[r] > 0 && cell.Cooldown <= 0f)
                            {
                                cell.Cooldown = 1.25f;
                                SpawnPea(r, c);
                            }
                            break;

                        case PlantType.Wallnut:
                            // no action
                            break;

                        case PlantType.Bomb:
                            // explode after delay (arm timer in AuxTimer)
                            if (!cell.Armed)
                            {
                                cell.Armed = true;
                                cell.AuxTimer = 1.2f;
                            }
                            else if (cell.AuxTimer <= 0f)
                            {
                                ExplodeBomb(r, c);
                                cell = PlantCell.Empty();
                            }
                            break;
                    }

                    // Dead plant?
                    if (cell.Type != PlantType.None && cell.HP <= 0)
                    {
                        SpawnParticles(CellCenterX(c), CellCenterY(r), 18, Color.FromArgb(180, 180, 180));
                        cell = PlantCell.Empty();
                    }
                }
            }
        }

        private void SpawnPea(int row, int col)
        {
            if (_projCount >= MaxProjectiles) return;

            float x = CellCenterX(col) + 12;
            float y = CellCenterY(row);

            _projs[_projCount++] = new Projectile
            {
                Alive = true,
                Row = row,
                X = x,
                Y = y,
                Vx = 260f,
                Damage = 20,
                Radius = 7f
            };
        }

        private void ExplodeBomb(int row, int col)
        {
            float cx = CellCenterX(col);
            float cy = CellCenterY(row);

            SpawnParticles(cx, cy, 45, Color.FromArgb(255, 150, 60));

            float radius = 1.35f * CellW;

            for (int i = 0; i < _zombieCount; i++)
            {
                if (!_zombies[i].Alive) continue;
                float dx = _zombies[i].X - cx;
                float dy = _zombies[i].Y - cy;
                float d2 = dx * dx + dy * dy;
                if (d2 <= radius * radius)
                {
                    _zombies[i].HP -= 180;
                    _zombies[i].HitFlash = 0.15f;
                }
            }
        }

        private static int GetPlantCost(PlantType t) => t switch
        {
            PlantType.PeaShooter => 100,
            PlantType.Sunflower => 50,
            PlantType.Wallnut => 50,
            PlantType.Bomb => 125,
            _ => 0
        };

        // ----------------------------
        // Zombies
        // ----------------------------
        private void SpawnZombies(float dt)
        {
            _spawnTimer -= dt;
            if (_spawnTimer > 0f) return;

            _spawnTimer = _spawnInterval;

            // spawn on right edge, random row
            int row = _rng.Next(Rows);

            var type = RollZombieType();
            float x = LawnLeft + Cols * CellW + 40;
            float y = CellCenterY(row);

            if (_zombieCount >= MaxZombies) return;

            var z = Zombie.Create(type, row, x, y);

            // small randomization
            z.X += _rng.Next(-10, 11);
            z.Speed *= 0.92f + (float)_rng.NextDouble() * 0.18f;

            _zombies[_zombieCount++] = z;
        }

        private ZombieType RollZombieType()
        {
            // More tough types as wave increases
            int w = _wave;
            int roll = _rng.Next(100);

            if (w < 2)
                return roll < 80 ? ZombieType.Basic : ZombieType.Fast;

            if (w < 5)
                return roll < 60 ? ZombieType.Basic : (roll < 85 ? ZombieType.Fast : ZombieType.Tank);

            // Later
            return roll < 45 ? ZombieType.Basic : (roll < 70 ? ZombieType.Fast : (roll < 90 ? ZombieType.Tank : ZombieType.Armored));
        }

        private void UpdateZombies(float dt)
        {
            for (int i = 0; i < _zombieCount; i++)
            {
                if (!_zombies[i].Alive) continue;

                ref var z = ref _zombies[i];

                z.HitFlash -= dt;
                if (z.HitFlash < 0) z.HitFlash = 0;

                // If currently attacking, don't walk
                if (z.Attacking)
                {
                    z.AttackCooldown -= dt;
                    continue;
                }

                // walk left
                z.X -= z.Speed * dt;
                z.Y = CellCenterY(z.Row); // locked to lane center
            }
        }

        // ----------------------------
        // Projectiles
        // ----------------------------
        private void UpdateProjectiles(float dt)
        {
            for (int i = 0; i < _projCount; i++)
            {
                if (!_projs[i].Alive) continue;

                ref var p = ref _projs[i];

                p.X += p.Vx * dt;

                // kill if out of bounds
                if (p.X > LawnLeft + Cols * CellW + 120)
                    p.Alive = false;
            }
        }

        // ----------------------------
        // Combat resolution (procedural + switch )
        // ----------------------------
        private void ResolveCombat(float dt)
        {
            // 1) Projectiles hit zombies
            for (int pi = 0; pi < _projCount; pi++)
            {
                if (!_projs[pi].Alive) continue;
                ref var p = ref _projs[pi];

                // Find nearest zombie in same row, in front of pea
                int hitZi = -1;
                float bestX = float.MaxValue;

                for (int zi = 0; zi < _zombieCount; zi++)
                {
                    if (!_zombies[zi].Alive) continue;
                    ref var z = ref _zombies[zi];

                    if (z.Row != p.Row) continue;
                    if (z.X < p.X) continue; // only forward hits

                    // collision test
                    float dx = (z.X - p.X);
                    float dist = Math.Abs(dx);

                    float hitWidth = GetZombieRadius(z.Type);

                    if (dist <= (p.Radius + hitWidth))
                    {
                        if (z.X < bestX)
                        {
                            bestX = z.X;
                            hitZi = zi;
                        }
                    }
                }

                if (hitZi >= 0)
                {
                    // apply damage
                    ApplyProjectileDamage(ref _zombies[hitZi], p.Damage);

                    _projs[pi].Alive = false;
                    SpawnParticles(p.X, p.Y, 6, Color.FromArgb(120, 220, 120));
                }
            }

            // 2) Zombies attack plants if overlapping a cell
            for (int zi = 0; zi < _zombieCount; zi++)
            {
                if (!_zombies[zi].Alive) continue;
                ref var z = ref _zombies[zi];

                int col = WorldXToCol(z.X);
                if (col < 0 || col >= Cols)
                {
                    z.Attacking = false;
                    continue;
                }

                ref var cell = ref _plants[z.Row, col];

                if (cell.Type == PlantType.None)
                {
                    // no plant to bite
                    z.Attacking = false;
                    z.AttackCooldown = 0;
                    continue;
                }

                // Are we actually "inside" the plant's cell?
                float plantX = CellCenterX(col);

                // If close enough, attack; else keep walking.
                if (Math.Abs(z.X - plantX) <= CellW * 0.25f)
                {
                    z.Attacking = true;

                    if (z.AttackCooldown <= 0f)
                    {
                        z.AttackCooldown = z.AttackPeriod;
                        int dmg = z.Damage;

                        // Some types have special behavior (switch)
                        switch (z.Type)
                        {
                            case ZombieType.Basic:
                                break;
                            case ZombieType.Fast:
                                // slightly lower damage but more frequent attacks already set in Create()
                                break;
                            case ZombieType.Tank:
                                // heavy bite
                                dmg += 6;
                                break;
                            case ZombieType.Armored:
                                // armor pierces wallnut a bit
                                if (cell.Type == PlantType.Wallnut) dmg += 8;
                                break;
                        }

                        cell.HP -= dmg;
                        SpawnParticles(plantX, CellCenterY(z.Row), 3, Color.FromArgb(210, 210, 210));
                    }
                }
                else
                {
                    z.Attacking = false;
                }
            }

            // 3) Cleanup dead zombies (keep array compact-ish)
            for (int zi = 0; zi < _zombieCount; zi++)
            {
                if (_zombies[zi].Alive && _zombies[zi].HP <= 0)
                {
                    _zombies[zi].Alive = false;
                    SpawnParticles(_zombies[zi].X, _zombies[zi].Y, 20, Color.FromArgb(240, 90, 90));
                }
            }
        }

        private static void ApplyProjectileDamage(ref Zombie z, int damage)
        {
            int finalDamage = damage;

            switch (z.Type)
            {
                case ZombieType.Basic:
                    break;

                case ZombieType.Fast:
                    // fast zombies are a bit squishier
                    finalDamage = (int)(damage * 1.15);
                    break;

                case ZombieType.Tank:
                    // tanks reduce damage
                    finalDamage = (int)(damage * 0.70);
                    break;

                case ZombieType.Armored:
                    // armored reduces even more (and has armor HP)
                    // Armor is a separate pool; when depleted, damage goes to HP.
                    int absorbed = Math.Min(z.ArmorHP, finalDamage);
                    z.ArmorHP -= absorbed;
                    finalDamage -= absorbed;

                    // After armor breaks, take remaining damage.
                    break;
            }

            z.HP -= finalDamage;
            z.HitFlash = 0.12f;
        }

        private static float GetZombieRadius(ZombieType t) => t switch
        {
            ZombieType.Basic => -18f,
            ZombieType.Fast => 16f,
            ZombieType.Tank => 22f,
            ZombieType.Armored => 18f, 
            _ => 18f
        };

        // ----------------------------
        // Particles (visual feedback)
        // ----------------------------
        private void SpawnParticles(float x, float y, int count, Color color)
        {
            for (int i = 0; i < count; i++)
            {
                if (_particleCount >= MaxParticles) break;

                float a = (float)(_rng.NextDouble() * Math.PI * 2);
                float sp = 60f + (float)_rng.NextDouble() * 140f;

                _particles[_particleCount++] = new Particle
                {
                    Alive = true,
                    X = x,
                    Y = y,
                    Vx = (float)Math.Cos(a) * sp,
                    Vy = (float)Math.Sin(a) * sp,
                    Life = 0.45f + (float)_rng.NextDouble() * 0.45f,
                    Color = color,
                    Size = 2f + (float)_rng.NextDouble() * 3.5f
                };
            }
        }

        private void UpdateParticles(float dt)
        {
            for (int i = 0; i < _particleCount; i++)
            {
                if (!_particles[i].Alive) continue;

                ref var p = ref _particles[i];

                p.Life -= dt;
                if (p.Life <= 0f)
                {
                    p.Alive = false;
                    continue;
                }

                p.X += p.Vx * dt;
                p.Y += p.Vy * dt;
                p.Vx *= (float)Math.Pow(0.20, dt); // quick damping
                p.Vy *= (float)Math.Pow(0.20, dt);
            }
        }

        // ----------------------------
        // Drawing
        // ----------------------------
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawLawn(g);
            DrawPlants(g);
            DrawZombies(g);
            DrawProjectiles(g);
            DrawParticles(g);
            DrawUI(g);

            if (_lost)
            {
                using var font = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold);
                var s = "YOU LOST (R to restart)";
                var sz = g.MeasureString(s, font);
                g.DrawString(s, font, Brushes.IndianRed,
                    LawnLeft + Cols * CellW / 2f - sz.Width / 2f,
                    LawnTop + Rows * CellH / 2f - sz.Height / 2f);
            }
        }

        private void DrawLawn(Graphics g)
        {
            using var cellPen = new Pen(Color.FromArgb(45, 52, 56), 1f);
            using var lawnBrush = new SolidBrush(Color.FromArgb(34, 60, 40));
            using var lawnBrush2 = new SolidBrush(Color.FromArgb(28, 54, 36));

            // background lawn
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var rect = CellRect(r, c);
                    g.FillRectangle(((r + c) % 2 == 0) ? lawnBrush : lawnBrush2, rect);
                    g.DrawRectangle(cellPen, rect);
                }
            }

            // spawn edge
            using var spawnPen = new Pen(Color.FromArgb(120, 120, 120), 2f);
            g.DrawLine(spawnPen,
                LawnLeft + Cols * CellW + 10, LawnTop,
                LawnLeft + Cols * CellW + 10, LawnTop + Rows * CellH);
        }

        private void DrawPlants(Graphics g)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var cell = _plants[r, c];
                    if (cell.Type == PlantType.None) continue;

                    float cx = CellCenterX(c);
                    float cy = CellCenterY(r);

                    switch (cell.Type)
                    {
                        case PlantType.Sunflower:
                            DrawCircle(g, cx, cy, 22, Color.FromArgb(245, 205, 70));
                            DrawCircle(g, cx, cy, 10, Color.FromArgb(150, 90, 30));
                            break;

                        case PlantType.PeaShooter:
                            DrawCircle(g, cx - 5, cy, 20, Color.FromArgb(90, 200, 90));
                            DrawCircle(g, cx + 16, cy - 2, 8, Color.FromArgb(70, 160, 70));
                            break;

                        case PlantType.Wallnut:
                            DrawRoundedRect(g, cx - 22, cy - 28, 44, 56, 12, Color.FromArgb(150, 110, 70));
                            break;

                        case PlantType.Bomb:
                            DrawCircle(g, cx, cy, 20, Color.FromArgb(60, 60, 60));
                            DrawCircle(g, cx + 10, cy - 12, 6, Color.FromArgb(220, 80, 60));
                            break;
                    }

                    // HP bar
                    DrawHpBar(g, cx - 30, cy + 32, 60, 6, cell.HP, cell.MaxHP);
                }
            }
        }

        private void DrawZombies(Graphics g)
        {
            for (int i = 0; i < _zombieCount; i++)
            {
                if (!_zombies[i].Alive) continue;

                var z = _zombies[i];

                Color body = z.Type switch
                {
                    ZombieType.Basic => Color.FromArgb(140, 170, 150),
                    ZombieType.Fast => Color.FromArgb(150, 190, 160),
                    ZombieType.Tank => Color.FromArgb(120, 150, 135),
                    ZombieType.Armored => Color.FromArgb(120, 150, 170),
                    _ => Color.FromArgb(140, 170, 150)
                };

                if (z.HitFlash > 0)
                    body = Color.FromArgb(210, 90, 90);

                float r = GetZombieRadius(z.Type);
                DrawCircle(g, z.X, z.Y, r, body);

                // simple "head"
                DrawCircle(g, z.X + r * 0.45f, z.Y - r * 0.35f, r * 0.35f, Color.FromArgb(210, 210, 210));

                // armor overlay
                if (z.Type == ZombieType.Armored && z.ArmorHP > 0)
                {
                    DrawRoundedRect(g, z.X - r, z.Y - r, r * 2, r * 2, 8, Color.FromArgb(80, 90, 110));
                    DrawHpBar(g, z.X - 20, z.Y - 34, 40, 5, z.ArmorHP, z.MaxArmorHP);
                }

                DrawHpBar(g, z.X - 24, z.Y + 26, 48, 6, z.HP, z.MaxHP);
            }
        }

        private void DrawProjectiles(Graphics g)
        {
            for (int i = 0; i < _projCount; i++)
            {
                if (!_projs[i].Alive) continue;
                var p = _projs[i];
                DrawCircle(g, p.X, p.Y, p.Radius, Color.FromArgb(120, 220, 120));
            }
        }

        private void DrawParticles(Graphics g)
        {
            for (int i = 0; i < _particleCount; i++)
            {
                if (!_particles[i].Alive) continue;
                var p = _particles[i];

                using var b = new SolidBrush(Color.FromArgb(
                    Math.Clamp((int)(p.Color.A), 0, 255),
                    p.Color.R, p.Color.G, p.Color.B));

                g.FillEllipse(b, p.X - p.Size * 0.5f, p.Y - p.Size * 0.5f, p.Size, p.Size);
            }
        }

        private void DrawUI(Graphics g)
        {
            float panelX = LawnLeft + Cols * CellW + 30;
            float panelY = LawnTop;

            using var font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            using var small = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular);

            // info
            g.DrawString($"Sun: {_sun}", font, Brushes.Gold, panelX, panelY);
            g.DrawString($"Wave: {_wave}", font, Brushes.White, panelX, panelY + 22);
            g.DrawString($"Spawn every: {_spawnInterval:F2}s", small, Brushes.Gainsboro, panelX, panelY + 45);
            g.DrawString($"Fast: {(_fastForward ? "ON" : "OFF")} (Space)", small, Brushes.Gainsboro, panelX, panelY + 63);

            // selection
            g.DrawString("Select plant:", font, Brushes.White, panelX, panelY + 95);

            DrawPlantButton(g, panelX, panelY + 120, PlantType.PeaShooter, "1", GetPlantCost(PlantType.PeaShooter));
            DrawPlantButton(g, panelX, panelY + 170, PlantType.Sunflower, "2", GetPlantCost(PlantType.Sunflower));
            DrawPlantButton(g, panelX, panelY + 220, PlantType.Wallnut, "3", GetPlantCost(PlantType.Wallnut));
            DrawPlantButton(g, panelX, panelY + 270, PlantType.Bomb, "4", GetPlantCost(PlantType.Bomb));

            g.DrawString("Click a cell to place.", small, Brushes.Gainsboro, panelX, panelY + 330);
            g.DrawString("R = restart", small, Brushes.Gainsboro, panelX, panelY + 350);
        }

        private void DrawPlantButton(Graphics g, float x, float y, PlantType t, string key, int cost)
        {
            var rect = new RectangleF(x, y, 230, 40);
            bool sel = _selectedPlant == t;

            using var b = new SolidBrush(sel ? Color.FromArgb(70, 90, 120) : Color.FromArgb(40, 45, 50));
            using var p = new Pen(sel ? Color.FromArgb(150, 190, 250) : Color.FromArgb(80, 90, 100), 1.5f);

            g.FillRoundedRectangle(b, rect, 10);
            g.DrawRoundedRectangle(p, rect, 10);

            string name = t.ToString();
            string text = $"{key}: {name}  ({cost})";
            using var font = new Font(FontFamily.GenericSansSerif, 10, sel ? FontStyle.Bold : FontStyle.Regular);
            g.DrawString(text, font, Brushes.White, x + 10, y + 11);
        }

        // ----------------------------
        // Geometry helpers
        // ----------------------------
        private (bool hit, int r, int c) ScreenToCell(Point p)
        {
            int x = p.X - LawnLeft;
            int y = p.Y - LawnTop;
            if (x < 0 || y < 0) return (false, 0, 0);

            int c = x / CellW;
            int r = y / CellH;

            if (c < 0 || c >= Cols || r < 0 || r >= Rows) return (false, 0, 0);
            return (true, r, c);
        }

        private static Rectangle CellRect(int r, int c)
            => new Rectangle(LawnLeft + c * CellW, LawnTop + r * CellH, CellW, CellH);

        private static float CellCenterX(int c) => LawnLeft + c * CellW + CellW * 0.5f;
        private static float CellCenterY(int r) => LawnTop + r * CellH + CellH * 0.5f;

        private static int WorldXToCol(float x)
        {
            int c = (int)((x - LawnLeft) / CellW);
            return c;
        }

        // ----------------------------
        // Drawing primitives
        // ----------------------------
        private static void DrawCircle(Graphics g, float x, float y, float radius, Color color)
        {
            using var b = new SolidBrush(color);
            g.FillEllipse(b, x - radius, y - radius, radius * 2, radius * 2);
        }

        private static void DrawRoundedRect(Graphics g, float x, float y, float w, float h, float r, Color color)
        {
            using var b = new SolidBrush(color);
            var rect = new RectangleF(x, y, w, h);
            g.FillRoundedRectangle(b, rect, r);
        }

        private static void DrawHpBar(Graphics g, float x, float y, float w, float h, int hp, int maxHp)
        {
            if (maxHp <= 0) return;
            float t = Math.Clamp(hp / (float)maxHp, 0f, 1f);

            using var bg = new SolidBrush(Color.FromArgb(60, 60, 60));
            using var fg = new SolidBrush(Color.FromArgb(110, 220, 110));
            using var bd = new Pen(Color.FromArgb(25, 25, 25), 1f);

            g.FillRectangle(bg, x, y, w, h);
            g.FillRectangle(fg, x, y, w * t, h);
            g.DrawRectangle(bd, x, y, w, h);
        }
    }

    // ----------------------------
    // Data-only structs (no OOP behavior)
    // ----------------------------
    public enum PlantType
    {
        None = 0,
        Sunflower = 1,
        PeaShooter = 2,
        Wallnut = 3,
        Bomb = 4
    }

    public struct PlantCell
    {
        public PlantType Type;
        public int HP;
        public int MaxHP;
        public float Cooldown;
        public float AuxTimer;
        public bool Armed;

        public static PlantCell Empty() => new PlantCell { Type = PlantType.None };

        public static PlantCell Create(PlantType t)
        {
            // More config
            switch (t)
            {
                case PlantType.Sunflower:
                    return new PlantCell { Type = t, HP = 70, MaxHP = 70, AuxTimer = 2.5f };
                case PlantType.PeaShooter:
                    return new PlantCell { Type = t, HP = 90, MaxHP = 90, Cooldown = 0.6f };
                case PlantType.Wallnut:
                    return new PlantCell { Type = t, HP = 340, MaxHP = 340 };
                case PlantType.Bomb:
                    return new PlantCell { Type = t, HP = 60, MaxHP = 60, Armed = false, AuxTimer = 0f };
                default:
                    return Empty();
            }
        }
    }

    public enum ZombieType
    {
        Basic = 0,
        Fast = 1,
        Tank = 2,
        Armored = 3
    }

    public struct Zombie
    {
        public bool Alive;

        public ZombieType Type;
        public int Row;

        public float X;
        public float Y;

        public int HP;
        public int MaxHP;

        public int ArmorHP;
        public int MaxArmorHP;

        public float Speed;

        public int Damage;
        public float AttackPeriod;
        public float AttackCooldown;
        public bool Attacking;

        public float HitFlash;

        public static Zombie Create(ZombieType t, int row, float x, float y)
        {
            var z = new Zombie
            {
                Alive = true,
                Type = t,
                Row = row,
                X = x,
                Y = y,
                Attacking = false,
                AttackCooldown = 0,
                HitFlash = 0
            };

            switch (t)
            {
                case ZombieType.Basic:
                    z.MaxHP = z.HP = 180;
                    z.Speed = 28f;
                    z.Damage = 12;
                    z.AttackPeriod = 0.95f;
                    z.MaxArmorHP = z.ArmorHP = 0;
                    break;

                case ZombieType.Fast:
                    z.MaxHP = z.HP = 135;
                    z.Speed = 44f;
                    z.Damage = 9;
                    z.AttackPeriod = 0.75f;
                    z.MaxArmorHP = z.ArmorHP = 0;
                    break;

                case ZombieType.Tank:
                    z.MaxHP = z.HP = 360;
                    z.Speed = 18f;
                    z.Damage = 14;
                    z.AttackPeriod = 1.05f;
                    z.MaxArmorHP = z.ArmorHP = 0;
                    break;

                case ZombieType.Armored:
                    z.MaxHP = z.HP = 170;
                    z.Speed = 24f;
                    z.Damage = 11;
                    z.AttackPeriod = 0.95f;
                    z.MaxArmorHP = z.ArmorHP = 120;
                    break;
            }

            return z;
        }
    }

    public struct Projectile
    {
        public bool Alive;
        public int Row;
        public float X;
        public float Y;
        public float Vx;
        public int Damage;
        public float Radius;
    }

    public struct Particle
    {
        public bool Alive;
        public float X;
        public float Y;
        public float Vx;
        public float Vy;
        public float Life;
        public Color Color;
        public float Size;
    }

    // ----------------------------
    // Small extensions for rounded rect (minimal)
    // ----------------------------
    internal static class GfxExt
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float radius)
        {
            using var path = RoundedRectPath(rect, radius);
            g.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, RectangleF rect, float radius)
        {
            using var path = RoundedRectPath(rect, radius);
            g.DrawPath(pen, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRectPath(RectangleF r, float radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            float d = radius * 2;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
