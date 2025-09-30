using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class SympathyManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{

	public SympathyManager()
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(AMove), nameof(AMove.Begin)),
			prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AMove_Begin_Prefix)),
			postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AMove_Begin_Postfix))
		);
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
	private static void AMove_Begin_Prefix(State s, Combat c, AMove __instance, out int __state)
	{
		
		Ship shipCheck = __instance.targetPlayer ? s.ship : c.otherShip;

		__state = shipCheck.x;
	}
	
	private static void AMove_Begin_Postfix(AMove __instance, State s, Combat c, in int __state)
	{
		Ship shipCheck = __instance.targetPlayer ? s.ship : c.otherShip;
		Ship ship = __instance.targetPlayer ? c.otherShip : s.ship;
		if (shipCheck.x == __state)
			return;
		var stacks = ship.Get(ModEntry.Instance.SympathyStatus.Status);
		if (stacks <= 0)
			return;
		if (__instance.dir > 0)
		{
			c.Queue(new AMove {targetPlayer = !__instance.targetPlayer, dir = -ship.Get(ModEntry.Instance.SympathyStatus.Status)});
		}
		if (__instance.dir < 0)
		{
			c.Queue(new AMove {targetPlayer = !__instance.targetPlayer, dir = ship.Get(ModEntry.Instance.SympathyStatus.Status)});
		}
		
	}
}
