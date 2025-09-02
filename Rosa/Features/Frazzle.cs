using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class FrazzleManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{

	public FrazzleManager()
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnTurnEnd), (State state, Combat combat) =>
		{
			if (combat.isPlayerTurn)
			{
				var stacks = state.ship.Get(ModEntry.Instance.FrazzleStatus.Status);
				if (stacks <= 0)
					return;
				combat.Queue(new AHurt(){hurtShieldsFirst = true, targetPlayer = true, hurtAmount = stacks});
				combat.Queue(new AStatus(){status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = -1, targetPlayer = true});
			}
			else
			{
				var stacks = combat.otherShip.Get(ModEntry.Instance.FrazzleStatus.Status);
				if (stacks <= 0)
					return;
				combat.Queue(new AHurt(){hurtShieldsFirst = true, targetPlayer = false, hurtAmount = stacks});
				combat.Queue(new AStatus(){status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = -1, targetPlayer = false});
			}

		});
	}
}
