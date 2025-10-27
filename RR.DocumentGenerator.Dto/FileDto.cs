namespace RR.DocumentGenerator.Dto
{
    public class FileDto
    {
        public string Name { get; set; } = null!;

        public byte[] Content { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long Size { get; set; }
    }
}
