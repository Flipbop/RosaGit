using System.Collections.Generic;
using Nickel;
using System.Linq;
using FSPRO;
using Microsoft.Extensions.Logging;
using Shockah.Kokoro;

namespace Flipbop.Cleo;
internal sealed class ImpairedCostManager
{
    internal static readonly ICardTraitEntry Trait = ModEntry.Instance.ImpairedTrait;
	internal readonly IKokoroApi.IV2.IActionCostsApi.IHook Hook = new ImpairedCostHook();
    public ImpairedCostManager()
    {
        ModEntry.Instance.KokoroApi.ActionCosts.RegisterHook(Hook);
        ModEntry.Instance.KokoroApi.ActionCosts.RegisterResourceCostIcon(new ImpairedCost(), ModEntry.Instance.ImpairedIcon.Sprite, ModEntry.Instance.ImpairCostIcon.Sprite);
    }
}

internal sealed class ImpairedCost : IKokoroApi.IV2.IActionCostsApi.IResource
{
    public string ResourceKey => "Cleo::Impaired";
    public int GetCurrentResourceAmount(State state, Combat combat)
    {
        int index = combat.hand.Count -1;
        int upgradeCounter = 0;
        int? currentCard = ModEntry.Instance.helper.ModData.ObtainModData<int?>(combat, "Card");
        while (index >= 0)
        {
            if (combat.hand[index].uuid != currentCard)
            {
	            if (combat.hand[index].upgrade != Upgrade.None)
	            {
		            upgradeCounter++;
	            }
            }
            index--;
        }
        return upgradeCounter;
    }

    public void Pay(State s, Combat c, int amount)
    {
        int index = c.hand.Count -1;
	    while (index >= 0 && amount > 0)
	    {
		    if (c.hand[index].upgrade != Upgrade.None)
		    {
			    if (!c.hand[index].GetIsImprovedA() && !c.hand[index].GetIsImprovedB())
			    {
				    ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImpairedTrait, true, false);
				    ImpairedExt.AddImpaired(c.hand[index], s);
			    }
			    else
			    {
				    ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImprovedATrait, false, false);
				    ImprovedAExt.RemoveImprovedA(c.hand[index], s);
				    ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImprovedBTrait, false, false);
				    ImprovedBExt.RemoveImprovedB(c.hand[index], s);
			    }
			    amount--;
			    Audio.Play(Event.CardHandling);
			    if (s.EnumerateAllArtifacts().Any((a) => a is CleoDrakeArtifact))
			    {
				    c.Queue(new AStatus { targetPlayer = true, status = Status.heat, statusAmount = 1 });
			    }
			    if (s.EnumerateAllArtifacts().Any((a) => a is CleoDizzyArtifact))
			    {
				    c.Queue(new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 1 });
				    if (c.hand[index].GetMeta().deck == Deck.dizzy)
				    {
					    c.Queue(new AImproveASelf() { id = c.hand[index].uuid });
				    }
			    }
		    }
		    index--;
	    }
    }

    public IReadOnlyList<Tooltip> GetTooltips(State state, Combat combat, int amount)
	    => [
		    new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Impair")
		    {
			    Icon = ModEntry.Instance.ImpairedIcon.Sprite,
			    TitleColor = Colors.action,
			    Title = ModEntry.Instance.Localizations.Localize(["action", "ImpairCost", "name"]),
			    Description = ModEntry.Instance.Localizations.Localize(["action", "ImpairCost", "description"])
		    }
	    ];
}

internal sealed class ImpairedCostHook : IKokoroApi.IV2.IActionCostsApi.IHook
{
	public bool ModifyActionCost(IKokoroApi.IV2.IActionCostsApi.IHook.IModifyActionCostArgs args)
    {
        ModEntry.Instance.helper.ModData.SetModData(args.Combat, "Card", args.Card?.uuid);
        return false;
    }
}
