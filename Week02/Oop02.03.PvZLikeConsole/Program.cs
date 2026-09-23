// VOB - Exercise 2, block 5 (cross-platform variant)
// The same PvZ-style game as the WinForms one, drawn in the console,
// so it runs on Linux and macOS as well. Written procedurally, on purpose.
// One static class holds the whole world: state, rules, input and drawing.
//
// Controls:
// - arrows or W A S D move the cursor
// - 1 = PeaShooter, 2 = Sunflower, 3 = Wallnut, 4 = Bomb
// - Enter or Space plants the selected plant, R restarts, Q quits
//
// Rules (simplified):
// - Sunflowers generate sun, peashooters shoot right, wallnuts block,
//   bombs explode after a delay. Zombies walk left and eat plants.
// - You lose when a zombie reaches the left edge.
//
// It runs. It is also a good example of what happens when nobody
// draws the classes first - see the guide for what to do with it.

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PvZLikeConsole
{
    // Shared mutable state, reachable from anywhere in the program.
    internal static class G
    {
        public static int Sun = 50;
        public static int Wave = 0;
        public static float Time = 0f;
        public static bool Lost = false;
    }

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

        public static PlantCell Empty()
        {
            return new PlantCell { Type = PlantType.None };
        }

        public static PlantCell Create(PlantType t)
        {
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
        public float X;              // in cells, 0 = leftmost column

        public int HP;
        public int MaxHP;
        public int ArmorHP;

        public float Speed;          // cells per second
        public int Damage;
        public float AttackPeriod;
        public float AttackCooldown;
        public bool Attacking;

        public static Zombie Create(ZombieType t, int row, float x)
        {
            Zombie z = new Zombie
            {
                Alive = true,
                Type = t,
                Row = row,
                X = x,
                Attacking = false,
                AttackCooldown = 0f
            };

            switch (t)
            {
                case ZombieType.Basic:
                    z.HP = 100; z.MaxHP = 100; z.Speed = 0.45f;
                    z.Damage = 10; z.AttackPeriod = 1.0f;
                    break;
                case ZombieType.Fast:
                    z.HP = 70; z.MaxHP = 70; z.Speed = 0.80f;
                    z.Damage = 7; z.AttackPeriod = 0.7f;
                    break;
                case ZombieType.Tank:
                    z.HP = 260; z.MaxHP = 260; z.Speed = 0.28f;
                    z.Damage = 16; z.AttackPeriod = 1.3f;
                    break;
                case ZombieType.Armored:
                    z.HP = 120; z.MaxHP = 120; z.ArmorHP = 90; z.Speed = 0.40f;
                    z.Damage = 12; z.AttackPeriod = 1.0f;
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
        public float Vx;
        public int Damage;
    }

    internal static class Program
    {
        // ----------------------------
        // "Global" state (static-ish)
        // ----------------------------
        private const int Rows = 5;
        private const int Cols = 9;

        private const int MaxZombies = 128;
        private const int MaxProjectiles = 256;

        private const float BaseDt = 0.1f;          // ten simulation steps per second

        private static readonly Random _rng = new Random(12345);

        private static PlantCell[,] _plants = new PlantCell[Rows, Cols];

        private static Zombie[] _zombies = new Zombie[MaxZombies];
        private static int _zombieCount = 0;

        private static Projectile[] _projs = new Projectile[MaxProjectiles];
        private static int _projCount = 0;

        private static int[] _zombieCountInLane = new int[Rows];

        private static float _spawnTimer = 0f;
        private static float _spawnInterval = 2.2f;

        private static PlantType _selectedPlant = PlantType.PeaShooter;
        private static int _cursorRow = 2;
        private static int _cursorCol = 0;

        private static string _message = "";
        private static bool _quit = false;

        private static void Main()
        {
            Console.CursorVisible = false;
            Console.Clear();

            ResetGame();

            Stopwatch clock = Stopwatch.StartNew();
            double last = 0.0;
            float accum = 0f;

            while (!_quit)
            {
                HandleInput();

                double now = clock.Elapsed.TotalSeconds;
                float dt = (float)(now - last);
                last = now;

                if (dt > 0.25f) dt = 0.25f;

                accum += dt;
                while (accum >= BaseDt)
                {
                    Step(BaseDt);
                    accum -= BaseDt;
                }

                Draw();
                Thread.Sleep(20);
            }

            Console.CursorVisible = true;
            Console.WriteLine();
            Console.WriteLine("Bye.");
        }

        private static void ResetGame()
        {
            G.Sun = 150;
            G.Wave = 0;
            G.Time = 0f;
            G.Lost = false;

            _selectedPlant = PlantType.PeaShooter;
            _cursorRow = 2;
            _cursorCol = 0;
            _message = "";

            _plants = new PlantCell[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _plants[r, c] = PlantCell.Empty();

            _zombies = new Zombie[MaxZombies];
            _zombieCount = 0;

            _projs = new Projectile[MaxProjectiles];
            _projCount = 0;

            _spawnTimer = 2.0f;
            _spawnInterval = 2.2f;
        }

        // ----------------------------
        // Input
        // ----------------------------
        private static void HandleInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo k = Console.ReadKey(true);

                switch (k.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (_cursorRow > 0) _cursorRow--;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (_cursorRow < Rows - 1) _cursorRow++;
                        break;

                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (_cursorCol > 0) _cursorCol--;
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (_cursorCol < Cols - 1) _cursorCol++;
                        break;

                    case ConsoleKey.D1:
                        _selectedPlant = PlantType.PeaShooter;
                        break;
                    case ConsoleKey.D2:
                        _selectedPlant = PlantType.Sunflower;
                        break;
                    case ConsoleKey.D3:
                        _selectedPlant = PlantType.Wallnut;
                        break;
                    case ConsoleKey.D4:
                        _selectedPlant = PlantType.Bomb;
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        TryPlant(_cursorRow, _cursorCol);
                        break;

                    case ConsoleKey.R:
                        ResetGame();
                        break;

                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        _quit = true;
                        break;
                }
            }
        }

        private static void TryPlant(int row, int col)
        {
            if (G.Lost) return;

            if (_plants[row, col].Type != PlantType.None)
            {
                _message = "the cell is taken";
                return;
            }

            int cost = GetPlantCost(_selectedPlant);
            if (G.Sun < cost)
            {
                _message = "not enough sun";
                return;
            }

            G.Sun -= cost;
            _plants[row, col] = PlantCell.Create(_selectedPlant);
            _message = "";
        }

        private static int GetPlantCost(PlantType t)
        {
            switch (t)
            {
                case PlantType.PeaShooter: return 100;
                case PlantType.Sunflower: return 50;
                case PlantType.Wallnut: return 50;
                case PlantType.Bomb: return 125;
                default: return 0;
            }
        }

        // ----------------------------
        // Simulation
        // ----------------------------
        private static void Step(float dt)
        {
            if (G.Lost) return;

            G.Time += dt;

            // Make waves gradually harder
            if ((int)(G.Time / 25f) > G.Wave)
            {
                G.Wave = (int)(G.Time / 25f);
                _spawnInterval = Math.Max(0.75f, 2.2f - G.Wave * 0.18f);
            }

            // Recompute lane counts
            for (int r = 0; r < Rows; r++) _zombieCountInLane[r] = 0;
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
            UpdateProjectiles(dt);

            // Lose condition
            for (int i = 0; i < _zombieCount; i++)
            {
                if (_zombies[i].Alive && _zombies[i].X < -0.2f)
                {
                    G.Lost = true;
                    _message = "a zombie got through";
                    break;
                }
            }
        }

        // ----------------------------
        // Plants
        // ----------------------------
        private static void UpdatePlants(float dt)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    ref PlantCell cell = ref _plants[r, c];
                    if (cell.Type == PlantType.None) continue;

                    if (cell.Type == PlantType.Sunflower)
                        cell.Cooldown -= dt;

                    cell.AuxTimer -= dt;

                    switch (cell.Type)
                    {
                        case PlantType.Sunflower:
                            if (cell.AuxTimer <= 0f)
                            {
                                cell.AuxTimer = 6.0f;
                                G.Sun += 25;
                            }
                            break;

                        case PlantType.PeaShooter:
                            if (_zombieCountInLane[r] > 0 && cell.Cooldown <= 0f)
                            {
                                cell.Cooldown = 1.25f;
                                SpawnPea(r, c);
                            }
                            break;

                        case PlantType.Wallnut:
                            break;

                        case PlantType.Bomb:
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

                    if (cell.Type != PlantType.None && cell.HP <= 0)
                        cell = PlantCell.Empty();
                }
            }
        }

        private static void SpawnPea(int row, int col)
        {
            if (_projCount >= MaxProjectiles) return;

            _projs[_projCount] = new Projectile
            {
                Alive = true,
                Row = row,
                X = col + 0.5f,
                Vx = 4.0f,
                Damage = 20
            };

            _projCount++;
        }

        private static void ExplodeBomb(int row, int col)
        {
            float radius = 1.35f;

            for (int i = 0; i < _zombieCount; i++)
            {
                if (!_zombies[i].Alive) continue;

                float dx = _zombies[i].X - col;
                float dy = _zombies[i].Row - row;

                if (dx * dx + dy * dy <= radius * radius)
                    _zombies[i].HP -= 180;
            }
        }

        // ----------------------------
        // Zombies
        // ----------------------------
        private static void SpawnZombies(float dt)
        {
            _spawnTimer -= dt;
            if (_spawnTimer > 0f) return;

            _spawnTimer = _spawnInterval;

            if (_zombieCount >= MaxZombies) return;

            int row = _rng.Next(Rows);
            ZombieType type = RollZombieType();

            Zombie z = Zombie.Create(type, row, Cols + 0.5f);
            z.Speed *= 0.92f + (float)_rng.NextDouble() * 0.18f;

            _zombies[_zombieCount] = z;
            _zombieCount++;
        }

        private static ZombieType RollZombieType()
        {
            int w = G.Wave;
            int roll = _rng.Next(100);

            if (w < 2)
                return roll < 80 ? ZombieType.Basic : ZombieType.Fast;

            if (w < 5)
                return roll < 60 ? ZombieType.Basic : (roll < 85 ? ZombieType.Fast : ZombieType.Tank);

            return roll < 45 ? ZombieType.Basic
                 : (roll < 70 ? ZombieType.Fast
                 : (roll < 90 ? ZombieType.Tank : ZombieType.Armored));
        }

        private static void UpdateZombies(float dt)
        {
            for (int i = 0; i < _zombieCount; i++)
            {
                if (!_zombies[i].Alive) continue;

                ref Zombie z = ref _zombies[i];

                if (z.Attacking)
                {
                    z.AttackCooldown -= dt;
                    continue;
                }

                z.X -= z.Speed * dt;
            }
        }

        // ----------------------------
        // Projectiles
        // ----------------------------
        private static void UpdateProjectiles(float dt)
        {
            for (int i = 0; i < _projCount; i++)
            {
                if (!_projs[i].Alive) continue;

                ref Projectile p = ref _projs[i];
                p.X += p.Vx * dt;

                if (p.X > Cols + 1.5f)
                    p.Alive = false;
            }
        }

        // ----------------------------
        // Combat resolution (procedural + switch)
        // ----------------------------
        private static void ResolveCombat(float dt)
        {
            // 1) Projectiles hit zombies
            for (int pi = 0; pi < _projCount; pi++)
            {
                if (!_projs[pi].Alive) continue;

                int hitZi = -1;
                float bestX = float.MaxValue;

                for (int zi = 0; zi < _zombieCount; zi++)
                {
                    if (!_zombies[zi].Alive) continue;
                    if (_zombies[zi].Row != _projs[pi].Row) continue;
                    if (_zombies[zi].X < _projs[pi].X) continue;

                    float dist = Math.Abs(_zombies[zi].X - _projs[pi].X);

                    if (dist <= 0.45f && _zombies[zi].X < bestX)
                    {
                        bestX = _zombies[zi].X;
                        hitZi = zi;
                    }
                }

                if (hitZi >= 0)
                {
                    ApplyProjectileDamage(ref _zombies[hitZi], _projs[pi].Damage);
                    _projs[pi].Alive = false;
                }
            }

            // 2) Zombies attack plants
            for (int zi = 0; zi < _zombieCount; zi++)
            {
                if (!_zombies[zi].Alive) continue;

                ref Zombie z = ref _zombies[zi];

                int col = (int)Math.Floor(z.X);
                if (col < 0 || col >= Cols)
                {
                    z.Attacking = false;
                    continue;
                }

                ref PlantCell cell = ref _plants[z.Row, col];

                if (cell.Type == PlantType.None)
                {
                    z.Attacking = false;
                    z.AttackCooldown = 0f;
                    continue;
                }

                float plantX = col + 0.5f;

                if (Math.Abs(z.X - plantX) <= 0.3f)
                {
                    z.Attacking = true;

                    if (z.AttackCooldown <= 0f)
                    {
                        z.AttackCooldown = z.AttackPeriod;
                        int dmg = z.Damage;

                        switch (z.Type)
                        {
                            case ZombieType.Basic:
                                break;
                            case ZombieType.Fast:
                                break;
                            case ZombieType.Tank:
                                dmg += 6;
                                break;
                            case ZombieType.Armored:
                                if (cell.Type == PlantType.Wallnut) dmg += 8;
                                break;
                        }

                        cell.HP -= dmg;
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
                    _zombies[zi].Alive = false;

                // drop the dead one by moving the last one into its place
                if (!_zombies[zi].Alive)
                {
                    _zombies[zi] = _zombies[_zombieCount - 1];
                    _zombieCount--;
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
                    finalDamage = (int)(damage * 1.15);
                    break;

                case ZombieType.Tank:
                    finalDamage = (int)(damage * 0.70);
                    break;

                case ZombieType.Armored:
                    int absorbed = Math.Min(z.ArmorHP, finalDamage);
                    z.ArmorHP -= absorbed;
                    finalDamage -= absorbed;
                    break;
            }

            z.HP -= finalDamage;
        }

        // ----------------------------
        // Rendering
        // ----------------------------
        private const int CellW = 4;     // characters per cell

        private static char PlantChar(PlantType t)
        {
            switch (t)
            {
                case PlantType.Sunflower: return 'S';
                case PlantType.PeaShooter: return 'P';
                case PlantType.Wallnut: return 'W';
                case PlantType.Bomb: return 'B';
                default: return '.';
            }
        }

        private static char ZombieChar(ZombieType t)
        {
            switch (t)
            {
                case ZombieType.Fast: return 'f';
                case ZombieType.Tank: return 'T';
                case ZombieType.Armored: return 'A';
                default: return 'z';
            }
        }

        private static void Draw()
        {
            int width = Cols * CellW;
            StringBuilder sb = new StringBuilder();

            sb.Append("  PvZ-like  ").Append("sun: ").Append(G.Sun)
              .Append("   wave: ").Append(G.Wave)
              .Append("   time: ").Append((int)G.Time).Append("s      ")
              .Append('\n');

            sb.Append("  selected: ").Append(_selectedPlant)
              .Append(" (").Append(GetPlantCost(_selectedPlant)).Append(")            ")
              .Append('\n');

            sb.Append("  +").Append(new string('-', width)).Append("+\n");

            for (int r = 0; r < Rows; r++)
            {
                char[] lane = new char[width];
                for (int i = 0; i < width; i++) lane[i] = ' ';

                // plants
                for (int c = 0; c < Cols; c++)
                {
                    lane[c * CellW + 1] = PlantChar(_plants[r, c].Type);
                }

                // projectiles
                for (int i = 0; i < _projCount; i++)
                {
                    if (!_projs[i].Alive || _projs[i].Row != r) continue;
                    int x = (int)Math.Round(_projs[i].X * CellW);
                    if (x >= 0 && x < width) lane[x] = '-';
                }

                // zombies
                for (int i = 0; i < _zombieCount; i++)
                {
                    if (!_zombies[i].Alive || _zombies[i].Row != r) continue;
                    int x = (int)Math.Round(_zombies[i].X * CellW);
                    if (x >= 0 && x < width) lane[x] = ZombieChar(_zombies[i].Type);
                }

                char left = (r == _cursorRow) ? '>' : '|';
                sb.Append("  ").Append(left).Append(new string(lane)).Append("|\n");
            }

            sb.Append("  +").Append(new string('-', width)).Append("+\n");

            // cursor marker line
            char[] marks = new char[width];
            for (int i = 0; i < width; i++) marks[i] = ' ';
            marks[_cursorCol * CellW + 1] = '^';
            sb.Append("   ").Append(new string(marks)).Append('\n');

            sb.Append("  1 pea  2 sun  3 wall  4 bomb   arrows move   enter plants   r restart   q quit\n");
            sb.Append("  ").Append(G.Lost ? "GAME OVER - press r" : _message).Append("                              \n");

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());
        }
    }
}
