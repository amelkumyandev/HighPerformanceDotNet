namespace ChannelsRealTimeDataProcessing
{
    public class DataItem
    {
        public int Id { get; set; }
        public string RawData { get; set; }
        public string ProcessedDataStage1 { get; set; }
        public string ProcessedDataStage2 { get; set; }
    }
}
