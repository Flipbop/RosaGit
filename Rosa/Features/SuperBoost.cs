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
		ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(new Hook());
	}

	private sealed class Hook : IKokoroApi.IV2.IStatusLogicApi.IHook
	{
		public int ModifyStatusChange(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusChangeArgs args)
		{
			var isPlayerShip = args.Ship.isPlayerShip;
			if (args.Status != Status.shield || args.Status != Status.tempShield)
			{
				if (args.OldAmount >= args.NewAmount || args.NewAmount <= 0) return args.NewAmount;
				var superBoost = args.Ship.Get(ModEntry.Instance.SuperBoostStatus.Status);
				return superBoost + args.NewAmount;
			} else return args.NewAmount;
		}
	}
}
