namespace KapturePlugin
{
    public enum LootMessageType : ushort
    {
        System = 57, // Add, Remove, Purchase, Discard, Obtained from Desynth, Use Dye
        LocalPlayerUse = 2092,
        AddDesynthSell = 2105, // Add Orchestration, Desynth, Sell
        LocalPlayerObtainLoot = 2110,
        LocalPlayerRoll = 2113,
        FastCraft = 2114, // Attach Materia, Extract Materia, Quick Synth
        Gather = 2115,
        LocalPlayerSynthesize = 2242,
        LocalPlayerSpecialObtain = 2622,
        OtherPlayerObtainLoot = 4158,
        OtherPlayerRoll = 4161,
        OtherPlayerUse = 8236,
        OtherPlayerSynthesize = 8258
    }
}