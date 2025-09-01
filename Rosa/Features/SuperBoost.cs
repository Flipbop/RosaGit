using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class SuperBoostManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{

	public SuperBoostManager()
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.ModifyStatusAmount),
			(int baseAmount, Card card, State state, Combat? combat) =>
			{
				var stacks = state.ship.Get(ModEntry.Instance.SuperBoostStatus.Status);
				return stacks;
			});
	}
}
