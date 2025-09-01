using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class RebuttalManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{

	public RebuttalManager()
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnPlayerTakeNormalDamage),
			(State state, Combat combat) =>
			{
				var stacks = state.ship.Get(ModEntry.Instance.RebuttalStatus.Status);
				if (stacks <= 0)
					return;
				combat.Queue(new AStatus(){status = Status.drawNextTurn, statusAmount = stacks, targetPlayer = true});
			});
	}
}
