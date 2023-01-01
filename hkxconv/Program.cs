using hkxconv.cmd;
using System.CommandLine;

namespace hkxconv
{

    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Tool for convert HKX file to XML");


            var convertFormatOpt = new Option<ConvertFormat>(name: "-v", description: "Convert format.", getDefaultValue: () => ConvertFormat.xml);
            var verboseOpt = new Option<bool>(name: "--verbose", description: "verbose", getDefaultValue: () => false);
            var inputDirArg = new Argument<FileSystemInfo>("Input dir or file");
            var outputDirArg = new Argument<FileSystemInfo>("Output dir or file", getDefaultValue: () => new DirectoryInfo("."));

            var convertCommand = new Command("convert", "Convert hkx file to other format")
            {
                convertFormatOpt,
                verboseOpt,
                inputDirArg,
                outputDirArg,
            };

            rootCommand.Add(convertCommand);

            convertCommand.SetHandler((format, verbose, input, output) =>
            {
                cmd.Convert.ToFormat(input, output, format, verbose);
            }, convertFormatOpt, verboseOpt, inputDirArg, outputDirArg);

            return await rootCommand.InvokeAsync(args);
        }
    }
}