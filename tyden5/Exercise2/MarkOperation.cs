namespace PackagingCellCycle
{
    class MarkOperation : Operation
    {
        public string Text { get; }

        public MarkOperation(string name, string text) : base(name)
        {
            Text = text;
        }
    }
}