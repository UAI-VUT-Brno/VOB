Console.WriteLine("=== 02.04 – Naming conventions ===\n");

Demo_NamingAndSealed();

// ──────────────────────────────────────────────
// 4. Naming conventions reminder
// ──────────────────────────────────────────────
static void Demo_NamingAndSealed()
{
    Console.WriteLine("--- Naming conventions ---");
    // • Always end with Exception: PaymentFailedException, ConfigurationMissingException.
    // • Name after the condition, not the operation: UserNotFoundException, not GetUserFailedException.
    // • Use sealed unless you explicitly design for inheritance.
    // • Expose structured data as properties, not only in the message string.
    // • Consistent naming makes custom exceptions immediately recognisable and integrates
    //   naturally with BCL conventions.
    Console.WriteLine("✓ PaymentFailedException   — names the condition, ends with Exception, sealed");
    Console.WriteLine("✓ ConfigurationMissingException");
    Console.WriteLine("✗ GetUserFailedException   — names the operation, not the condition");
    Console.WriteLine("✗ UserError                — missing 'Exception' suffix");
    Console.WriteLine();
}
