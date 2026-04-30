// Immutabilní bankovní účet – vlastní immutabilní typ se sdíleným stavem
// dotnet run --project 17-ImmutableBankAccount

// Proč immutabilní typ místo zamykání?
//   Klasický přístup: jeden mutable objekt + lock kolem každé operace.
//   Immutabilní přístup: každá operace vrátí NOVOU instanci se změněným stavem.
//   Sdílený odkaz na aktuální instanci se pak atomicky přepíše pomocí CAS.
//
// Výhoda: čtení stavu nevyžaduje žádný zámek – snímek stavu je konzistentní
// v daném okamžiku a nikdy se pod čtenářem nezmění (jiné vlákno vytvoří nový
// objekt, nepřepíše existující).

using System.Collections.Immutable;

// ── Demo: souběžné vklady a výběry ────────────────────────────────────────
var holder = new AccountHolder(new BankAccount("CZ001", 1_000m));

var tasks = new List<Task>();

// 5 vláken provádí vklady
for (int i = 0; i < 5; i++)
{
    int n = i + 1;
    tasks.Add(Task.Run(() => holder.Apply(acc => acc.Deposit(200m, $"vklad-{n}"))));
}

// 3 vlákna provádí výběry
for (int i = 0; i < 3; i++)
{
    int n = i + 1;
    tasks.Add(Task.Run(() => holder.Apply(acc => acc.Withdraw(150m, $"výběr-{n}"))));
}

await Task.WhenAll(tasks);

var final = holder.Current;
Console.WriteLine($"Číslo účtu : {final.AccountNumber}");
Console.WriteLine($"Zůstatek   : {final.Balance:C}");  // 1000 + 5*200 – 3*150 = 1550
Console.WriteLine($"Transakcí  : {final.Transactions.Count}");
Console.WriteLine();

foreach (var tx in final.Transactions)
    Console.WriteLine($"  [{tx.Id,2}] {tx.Description,-12}  {tx.Amount,+10:C}");

// ── Supporting types ──────────────────────────────────────────────────────

// Transaction – immutabilní záznam jedné operace
record Transaction(int Id, decimal Amount, string Description);

// BankAccount – immutabilní typ; veškerý stav je readonly
// Deposit/Withdraw NEVRACEJÍ void – vrátí novou instanci s aktualizovaným stavem.
// Původní instance zůstane nezměněná → vlákna, která ji drží, vidí konzistentní
// snímek bez jakéhokoli zámku.
sealed record BankAccount(
    string AccountNumber,
    decimal Balance,
    ImmutableList<Transaction> Transactions)
{
    // Zkrácený konstruktor: nový účet bez transakcí
    public BankAccount(string accountNumber, decimal initialBalance)
        : this(accountNumber, initialBalance, ImmutableList<Transaction>.Empty) { }

    public BankAccount Deposit(decimal amount, string description)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        var tx = new Transaction(Transactions.Count + 1, amount, description);
        return this with
        {
            Balance = Balance + amount,
            Transactions = Transactions.Add(tx)
        };
    }

    public BankAccount Withdraw(decimal amount, string description)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (amount > Balance) throw new InvalidOperationException("Nedostatek prostředků.");
        var tx = new Transaction(Transactions.Count + 1, -amount, description);
        return this with
        {
            Balance = Balance - amount,
            Transactions = Transactions.Add(tx)
        };
    }
}

// AccountHolder – spravuje sdílený odkaz na aktuální immutabilní instanci.
//
// Klíčový problém: jak atomicky přepsat odkaz na novou instanci?
//   1. Volatile.Read  – zaručí čerstvé čtení odkazu (bez CPU registrové cache)
//   2. Interlocked.CompareExchange – atomicky přepíše odkaz POUZE pokud se
//      od doby čtení nezměnil (žádné jiné vlákno jej nepřepsalo).
//      Pokud CompareExchange selže (jiné vlákno bylo rychlejší), smyčka
//      přečte nový aktuální stav a pokus opakuje – optimistický lock-free přístup.
//
// Žádné vlákno nečeká na jiné → žádný deadlock, žádný konvoj (lock convoy).
sealed class AccountHolder
{
    private BankAccount _current;

    public AccountHolder(BankAccount initial) => _current = initial;

    // Čtení je bezpečné bez zámku – snímek je immutabilní a nikdy se nezmění.
    public BankAccount Current => Volatile.Read(ref _current);

    // Apply(func): vezme aktuální stav, zavolá func (vrátí nový stav),
    // atomicky přepíše odkaz. Při souběhu selže CAS a pokus se zopakuje.
    public BankAccount Apply(Func<BankAccount, BankAccount> update)
    {
        BankAccount snapshot, updated;
        do
        {
            snapshot = Volatile.Read(ref _current);
            updated  = update(snapshot);            // vypočítat nový stav
        }
        // Přepiš _current z snapshot na updated – ale jen pokud _current stále
        // == snapshot. Pokud jiné vlákno stihlo přepsat dříve, vrátí se stará
        // hodnota (≠ snapshot) a smyčka se zopakuje s čerstvým snímkem.
        while (!ReferenceEquals(
            Interlocked.CompareExchange(ref _current, updated, snapshot),
            snapshot));

        return updated;
    }
}
