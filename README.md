[![Build status](https://ci.appveyor.com/api/projects/status/github/programmersdigest/MT940Parser?branch=release&svg=true)](https://ci.appveyor.com/api/projects/status/github/programmersdigest/MT940Parser?branch=release&svg=true)
# MT940Parser
A parser for the SWIFT MT940/MT942 format

## Description
This project implements a parser for the *SWIFT MT940* and *MT942* formats used for electronic banking (e.g. for electronic statements). The result is parsed into a developer friendly object model for further processing or storing.

**Do note**: This project is part of a small housekeeping book I am currently implementing for my own personal use. It may therefore be incomplete and/or faulty. If you have corrections or suggestions, please let me know.

## Features
- Parses files in the SWIFT MT940 and MT942 formats (see Relevant Materials)
- Fast: 100.000 max-length statements in ca. 6 secs
- Low memory-footprint: streams input data and provides results on a per-statement basis (using IEnumerable)
- Should adhere to the specification
- Parsing errors will currently result in exceptions (which may be subject to change in the near future)
- Is largely unit tested (200+ tests)

## Usage
Grab the latest version from NuGet https://www.nuget.org/packages/programmersdigest.MT940Parser

```
// Just provide a file path...
using (var parser = new Parser(path)) {
    foreach (var statement in parser.Parse()) {
        // Do something
    }
}

// ...or a stream
using (var parser = new Parser(networkStream)) {
    foreach (var statement in parser.Parse()) {
        // Do something
    }
}
```

## Todos
- Parse special fields used in MT942 only (i.E. :34F:, :13D:, :90D:, :90C:)
- Provide Transaction Type ID Code (in field :61:) as enum
- Store parsing errors in each statement so that subsequent statements can still be processed
- Add comments on public members

## Relevant Materials
https://deutschebank.nl/nl/docs/MT94042_EN.pdf  
https://www.bexio.com/files/content/SEO-lp/MT940/ZKB-MT940.pdf (GER)  
https://www.kontopruef.de/mt940s.shtml (GER)  
http://www.ebics.de/spezifikation/dfue-abkommen-anlage-3-formatstandards/ (see "Appendix 3 Data Formats V3.1.pdf" ch. 8)
https://www.handelsbanken.se/shb/inet/icentsv.nsf/vlookuppics/a_filmformatbeskrivningar_fil_mt940_account_statement_20081212/$file/mt940_account_statement.pdf (especially definition for structured data in field :86:)
