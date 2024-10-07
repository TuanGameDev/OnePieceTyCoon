Shonen Jump's One Piece (GBA) sounds and musics.

Ripped by G.o.D. .
To be hosted on Sprite Database: http://spritedatabase.net/
Credits are appreciated but not required.

Formats:
musics = .ogg files
sounds = .wav files




Ripping method = GBAMusicRipper    http://www.romhacking.net/utilities/881/
(it's actually called GBAMusRiper)

---

First of all you rip the sounds and musics in MIDIs, along with the soundfont/bank file.
The tool is to be used with the command prompt (hold the Windows Key and press r, then type "cmd" without the quotations marks to find it easily).
The readme tells you the commands you can use to rip specific files.
I recall having used the -raw command.

I typed
gba_mus_riper One_Piece.gba -raw
(instead of One_Piece.gba you can have the name of the rom you want to rip sounds from; also, before gba_mus_riper there should be the path to the directory in which the tool is located).
All the rips will be found in a folder called *name of rom* MIDI.

Then, you have to convert those with another program. I suggest you to use MIDI Converter Studio (http://www.maniactools.com/); be sure to use the Soundfont/bank that gets ripped by the GBAMusRiper.