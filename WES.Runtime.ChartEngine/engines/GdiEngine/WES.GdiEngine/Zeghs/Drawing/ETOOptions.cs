using System;

namespace Zeghs.Drawing {
	[Flags]
	internal enum ETOOptions : uint {
		ETO_CLIPPED = 0x4,
		ETO_GLYPH_INDEX = 0x10,
		ETO_IGNORELANGUAGE = 0x1000,
		ETO_NUMERICSLATIN = 0x800,
		ETO_NUMERICSLOCAL = 0x400,
		ETO_OPAQUE = 0x2,
		ETO_PDY = 0x2000,
		ETO_RTLREADING = 0x800,
	}
}