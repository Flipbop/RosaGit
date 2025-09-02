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
		
		
		
		
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnTurnEnd), (State state, Combat combat) =>
		{
			if (combat.isPlayerTurn)
			{
				var stacks = state.ship.Get(ModEntry.Instance.SympathyStatus.Status);
				if (stacks <= 0)
					return;
				combat.Queue(new AStatus(){status = ModEntry.Instance.SympathyStatus.Status, statusAmount = -1, targetPlayer = true});
			}
			else
			{
				var stacks = combat.otherShip.Get(ModEntry.Instance.SympathyStatus.Status);
				if (stacks <= 0)
					return;
				combat.Queue(new AStatus(){status = ModEntry.Instance.SympathyStatus.Status, statusAmount = -1, targetPlayer = false});
			}

		});
	}
}
