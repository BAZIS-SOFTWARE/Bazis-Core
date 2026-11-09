namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Индексы фаз «Структуры» для модели твердости Куркина. −1 — фазы нет в материале.
    /// </summary>
    public sealed class HardnessPhaseIndices
    {
        /// <summary>
        /// Создаёт набор индексов фаз.
        /// </summary>
        public HardnessPhaseIndices(int ferriteIndex, int perliteIndex, int beiniteIndex, int martensiteIndex, int austeniteIndex)
        {
            FerriteIndex = ferriteIndex;
            PerliteIndex = perliteIndex;
            BeiniteIndex = beiniteIndex;
            MartensiteIndex = martensiteIndex;
            AusteniteIndex = austeniteIndex;
        }

        public int FerriteIndex { get; }
        public int PerliteIndex { get; }
        public int BeiniteIndex { get; }
        public int MartensiteIndex { get; }
        public int AusteniteIndex { get; }
    }
}
