using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nickel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

public class BRINGITBACK : CardAction
{
	public override Route BeginWithRoute(G g, State s, Combat c)
	{
		timer = 0.0;
		return new ArtifactReward()
		{
			canSkip = false,
			artifacts = new List<Artifact>()
			{
				new BrokenGlasses()
			},
		};
	}
}
