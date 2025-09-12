using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace Flipbop.Rosa;

internal sealed class BrainFoodArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("BrainFood", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.RosaDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/BrainFood.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BrainFood", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BrainFood", "description"]).Localize
		});
		
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetDataWithOverrides)),
			postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ACost_Begin_Postfix)
			));
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(AEndTurn), nameof(AEndTurn.Begin)),
			prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AEndTurn_Begin_Prefix))
			);
	}
	
	private static void ACost_Begin_Postfix(ref CardData __result, Card __instance)
	{
		int baseCost = 0;
		if (ModEntry.Instance.helper.ModData.GetModDataOrDefault<int>(__instance, "BrainFoodBaseCost", 0) <= 0)
		{
			baseCost = __result.cost;
		}
		else baseCost = ModEntry.Instance.helper.ModData.GetModDataOrDefault<int>(__instance, "BrainFoodBaseCost", 0);
		__result.cost = baseCost + ModEntry.Instance.helper.ModData.GetModDataOrDefault<int>(__instance, "BrainFoodOffset", 0);
	}

	public override void OnCombatEnd(State state)
	{
		base.OnCombatEnd(state);
		foreach (Card card in state.deck)
		{
			ModEntry.Instance.helper.ModData.SetModData(card, "BrainFoodOffset", 0);
		}
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		base.OnCombatStart(state, combat);
		foreach (Card card in state.deck)
		{
			if (card.discount == 0) ModEntry.Instance.helper.ModData.SetModData(card, "BrainFoodBaseCost", card.GetCurrentCost(state));
			else   ModEntry.Instance.helper.ModData.SetModData(card, "BrainFoodBaseCost", card.GetCurrentCost(state)-card.discount);
		}
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
		int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		ModEntry.Instance.helper.ModData.SetModData(card, "BrainFoodOffset", ModEntry.Instance.helper.ModData.GetModData<int>(card, "BrainFoodOffset") + 1);
	}
	
	private static void AEndTurn_Begin_Prefix(State s, Combat c)
	{
		if (c.cardActions.Any(a => a is AEndTurn))
			return;
		foreach (Card card in c.hand)
		{
			ModEntry.Instance.helper.ModData.SetModData(card, "BrainFoodOffset", ModEntry.Instance.helper.ModData.GetModData<int>(card, "BrainFoodOffset") -1);
		} 
	}
}
