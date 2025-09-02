using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class SympathyManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{

	public SympathyManager()
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
	}
}
