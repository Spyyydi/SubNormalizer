# SubNormalizer
This program is made for normalizing subtitles converted from .ass format to .srt format.

In my work I convert .ass format to .srt format using [asstosrt-wasm](https://sorz.github.io/asstosrt-wasm/) site. However after formating there are lines that must be in one line but splited to multiple lines. By normalization it is ment to fix this problem.

Before I used [veed.io](https://www.veed.io/tools/subtitle-converter/ass-to-srt) site to edit .ass subtitles and convert to .srt format. Because some timestamps in subtitles obtained from [asstosrt-wasm](https://sorz.github.io/asstosrt-wasm/) and [veed.io](https://www.veed.io/tools/subtitle-converter/ass-to-srt) differed for 1ms I also made synchronization of those subtitles (take timestamps from .srt file obtained from [veed.io](https://www.veed.io/tools/subtitle-converter/ass-to-srt) site and replace timestamps in .srt file obtained from [asstosrt-wasm](https://sorz.github.io/asstosrt-wasm/) site). 
Another functionality implemented is replacing noninformative text for humans with simple dash character.
For example: `{\shad2\bord1\fade(200,200)\fs18\3c&H04045E&\4c&H06064E&\fnTrebuchet MS\pos(156,320)}` replaced with `-`
It is implemented same as synchronization with checkboxes which means if you do not need this functionality then you can uncheck checkboxes and do only normalization.

---

