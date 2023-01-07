using HKX2;

namespace hkxconv.cmd
{
    public enum ConvertFormat
    {
        xml = 0,
        hkx = 1,
        //win32 = 2,
    }
    public class Convert
    {
        public static int ToFormat(FileSystemInfo input, FileSystemInfo output, ConvertFormat format, bool verbose, bool ignoreError)
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
                    retCode = ConvertTo(inFile, outFile, format, verbose, ignoreError);
                    break;
                case (FileInfo inFile, DirectoryInfo outDir):
                    retCode = TraverseDir(inFile, outDir, format, verbose, ignoreError);
                    break;
                case (DirectoryInfo inDir, DirectoryInfo outDir):
                    retCode = TraverseDir(inDir, outDir, format, verbose, ignoreError);

                    break;
                default:
                    break;
            }
            return retCode;
        }

        public static int TraverseDir(DirectoryInfo inDir, DirectoryInfo outDir, ConvertFormat format, bool verbose, bool ignoreError)
        {
            if (!outDir.Exists)
                Directory.CreateDirectory(outDir.Name);

            int retCode;
            foreach (var item in Directory.EnumerateFiles(inDir.ToString(), "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(inDir.FullName, item);
                var inFile = new FileInfo(Path.Combine(inDir.ToString(), relativePath));
                var outFile = new FileInfo(Path.Combine(outDir.ToString(), Path.ChangeExtension(relativePath, format.ToString())));

                retCode = ConvertTo(inFile, outFile, format, verbose, ignoreError);
                if (retCode != 0)
                {
                    return retCode;
                }

            }
            return 0;
        }

        public static int TraverseDir(FileInfo inFile, DirectoryInfo outDir, ConvertFormat format, bool verbose, bool ignoreError)
        {
            if (!outDir.Exists)
                Directory.CreateDirectory(outDir.Name);

            var outFile = new FileInfo(Path.Combine(outDir.ToString(), Path.ChangeExtension(inFile.Name, format.ToString())));
            return ConvertTo(inFile, outFile, format, verbose, ignoreError);
        }

        public static int ConvertTo(FileInfo inFile, FileInfo outFile, ConvertFormat format, bool verbose, bool ignoreError)
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
                ConvertFormat.hkx => HKXHeader.SkyrimSE(),
                _ => throw new NotImplementedException("no format found"),
            };

            if (format == ConvertFormat.xml && Path.HasExtension(".hkx"))
            {
                var br = new BinaryReaderEx(inFile.OpenRead());
                var des = new PackFileDeserializer();
                IHavokObject rootObject = des.Deserialize(br, ignoreError);

                var xs = new XmlSerializer();
                xs.Serialize(rootObject, header, outFile.Create());
                return 0;
            }
            else if (format == ConvertFormat.hkx && Path.HasExtension(".xml"))
            {
                var xd = new XmlDeserializer();
                IHavokObject rootObject = xd.Deserialize(inFile.OpenRead(), header);

                var bw = new BinaryWriterEx(outFile.Create());
                var s = new PackFileSerializer();
                s.Serialize(rootObject, bw, header);
                return 0;
            }

            return 0;
        }
    }
}
