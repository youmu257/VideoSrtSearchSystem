namespace Share.Tool.MySQL
{
    public class TwoModelData<K, V>
    {
        public K? ModelK { get; set; }

        public V? ModelV { get; set; }

        public TwoModelData(K modelK, V modelV)
        {
            ModelK = modelK;
            ModelV = modelV;
        }
    }
}
