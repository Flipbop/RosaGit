using FSPRO;
using Nickel;
using System.Collections.Generic;
using System.Linq;


namespace Flipbop.Cleo;

public sealed class AImproveAHand : DynamicWidthCardAction
{
	public int Amount;

	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = c.hand.Count -1;
		while (index >= 0)
		{
			if (c.hand[index].upgrade == Upgrade.None)
			{
				if (!c.hand[index].GetIsImpaired() && c.hand[index].IsUpgradable() && c.hand[index].GetMeta().deck != Deck.trash)
				{
					if (s.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyB))
					{
						ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImprovedBTrait, true, false);
						ImprovedBExt.AddImprovedB(c.hand[index], s);
					}
					else
					{
						ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImprovedATrait, true, false);
						ImprovedAExt.AddImprovedA(c.hand[index], s);
					}
				}
				else
				{
					ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImpairedTrait, false, false);
					ImpairedExt.RemoveImpaired(c.hand[index], s, true);
				}
				Amount--;
				Audio.Play(Event.CardHandling);
				if (s.EnumerateAllArtifacts().Any((a) => a is CleoDrakeArtifact))
				{
					c.Queue(new AStatus { targetPlayer = true, status = Status.heat, statusAmount = -1 });
				}
			}
			index--;
		}
	}

	public override Icon? GetIcon(State s)
	{
		if (s.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyB))
		{
			return new(ModEntry.Instance.ImproveBHandIcon.Sprite, null, Colors.textMain);
		}
		return new(ModEntry.Instance.ImproveAHandIcon.Sprite, null, Colors.textMain);
	}

	public override List<Tooltip> GetTooltips(State s)
	{
		if (s.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyB))
		{
			return [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve B Hand")
				{
					Icon = ModEntry.Instance.ImproveBHandIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveBHand", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveBHand", "description"])
				}
			];
		}
		return [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve A Hand")
			{
				Icon = ModEntry.Instance.ImproveAHandIcon.Sprite,
				TitleColor = Colors.action,
				Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveAHand", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveAHand", "description"])
			}
		];
	} 
}
