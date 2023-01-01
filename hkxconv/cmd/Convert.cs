using HKX2;

namespace hkxconv.cmd
{
    public enum ConvertFormat
    {
        xml = 0,
        //amd64 = 1,
        //win32 = 2,
    }
    public class Convert
    {
        public static int ToFormat(FileSystemInfo input, FileSystemInfo output, ConvertFormat format, bool verbose)
        {
            if (!input.Exists)
            {
                Console.WriteLine("Input file not found.");
                return -1;
            }
            int retCode = -1;
            switch ((input, output))
            {
                case (FileInfo inFile, FileInfo outFile):
                    retCode = ConvertHKX(inFile, outFile, format, verbose);
                    break;
                case (FileInfo inFile, DirectoryInfo outDir):
                    retCode = ConvertHKX(inFile, outDir, format, verbose);
                    break;
                case (DirectoryInfo inDir, DirectoryInfo outDir):
                    retCode = ConvertHKX(inDir, outDir, format, verbose);

                    break;
                default:
                    break;
            }
            return retCode;
        }

        public static int ConvertHKX(DirectoryInfo inDir, DirectoryInfo outDir, ConvertFormat format, bool verbose)
        {
            if (!outDir.Exists)
                Directory.CreateDirectory(outDir.Name);

            int retCode;
            foreach (var item in Directory.EnumerateFiles(inDir.ToString(), "*.hkx", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(inDir.FullName, item);
                var inFile = new FileInfo(Path.Combine(inDir.ToString(), relativePath));
                var outFile = new FileInfo(Path.Combine(outDir.ToString(), Path.ChangeExtension(relativePath, format.ToString())));

                retCode = ConvertHKX(inFile, outFile, format, verbose);
                if (retCode != 0)
                {
                    return retCode;
                }

            }
            return 0;
        }

        public static int ConvertHKX(FileInfo inFile, DirectoryInfo outDir, ConvertFormat format, bool verbose)
        {
            if (!outDir.Exists)
                Directory.CreateDirectory(outDir.Name);

            var outFile = new FileInfo(Path.Combine(outDir.ToString(), Path.ChangeExtension(inFile.Name, format.ToString())));
            return ConvertHKX(inFile, outFile, format, verbose);
        }

        public static int ConvertHKX(FileInfo inFile, FileInfo outFile, ConvertFormat format, bool verbose)
        {
            if (verbose)
                Console.WriteLine($"Convert {inFile} to {outFile}");
            if (!outFile.Exists)
                if (outFile.DirectoryName is null)
                    throw new Exception("DirectoryName is null?");
                else
                    Directory.CreateDirectory(outFile.DirectoryName);

            HKXHeader header = (format) switch
            {
                ConvertFormat.xml => HKXHeader.SkyrimSE(),
                _ => throw new NotImplementedException("no format found"),
            };

            var br = new BinaryReaderEx(inFile.OpenRead());
            var des = new PackFileDeserializer();
            IHavokObject rootObject = des.Deserialize(br);

            var xs = new XmlSerializer();

            xs.Serialize(rootObject, header, outFile.Create());

            return 0;
        }
    }
}
