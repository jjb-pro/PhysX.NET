namespace PhysX.Samples.VehicleSample;

public class VehicleFilterShader : SimulationFilterShader
{
    public override FilterResult Filter(int attributes0, FilterData filterData0, int attributes1, FilterData filterData1)
    {
        if (0 == (filterData0.Word0 & filterData1.Word1) && 0 == (filterData1.Word0 & filterData0.Word1))
            return new FilterResult() { FilterFlag = FilterFlag.Suppress };

        PairFlag pairFlags = PairFlag.ContactDefault;
        pairFlags |= (PairFlag)(ushort)(filterData0.Word2 | filterData1.Word2);

        return new FilterResult() { PairFlags = pairFlags };
    }
}