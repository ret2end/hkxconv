# hkxconv
a tool for convert skyrim SE hkx to xml 

```
Description:
  Tool for convert HKX file to XML

Usage:
  hkxconv [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  convert <Input dir or file> <Output dir or file>  Convert hkx file to other format [default: .]
```

## convert hkx to xml
`hkxconv.exe convert file_to_convert.hkx converted_filename.xml`

`hkxconv.exe convert file_to_convert.hkx .\hkx_out`

`hkxconv.exe convert .\meshes .\meshes_out`

```
Description:
  Convert hkx file to other format

Usage:
  hkxconv convert <Input dir or file> [<Output dir or file>] [options]

Arguments:
  <Input dir or file>
  <Output dir or file>  [default: .]

Options:
  -v <xml>        Convert format. [default: xml]
  --verbose       verbose [default: False]
  -?, -h, --help  Show help and usage information
```
