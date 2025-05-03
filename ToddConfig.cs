namespace Todd;

public class ToddConfig
{
    public class AssetConfig
    {
        public required string LargeImage { get; set; }
        public required string LargeImageText { get; set; }
        public required string SmallImage { get; set; }
    }

    public class ButtonConfig
    {
        public required string Label { get; set; }
        public required string Url { get; set; }
    }
    
    public static ToddConfig Default => new()
    {
        Details = "Todd Discord RPC",
        State = "Todding around",
        Assets = new AssetConfig
        {
            LargeImage = "https://picsum.photos/1024",
            LargeImageText = "A little image from https://picsum.photos",
            SmallImage = "https://picsum.photos/1024",
        },
        Buttons =
        [
            new ButtonConfig
            {
                Label = "Todd on GitHub",
                Url = "https://github.com/voidwyrm-2/Todd"
            }
        ]
    };
    
    public required string Details { get; set; }
    public required string State { get; set; }
    public required AssetConfig Assets { get; set; }
    public required ButtonConfig[] Buttons { get; set; }
}