using FSPRO;
using Nickel;
using System.Collections.Generic;
using System.Linq;


namespace Flipbop.Cleo;

public sealed class AImproveB : DynamicWidthCardAction
{
	public required int Amount;

	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = c.hand.Count -1;
		while (index >= 0 && Amount > 0)
		{
			if (c.hand[index].upgrade == Upgrade.None)
			{
				if (!c.hand[index].GetIsImpaired() && c.hand[index].IsUpgradable() && c.hand[index].GetMeta().deck != Deck.trash)
				{
					if (s.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyA))
					{
						ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImprovedATrait, true, false);
						ImprovedAExt.AddImprovedA(c.hand[index], s);
					}
					else
					{
						ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, c.hand[index], ModEntry.Instance.ImprovedBTrait, true, false);
						ImprovedBExt.AddImprovedB(c.hand[index], s);
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
		if (s.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyA))
		{
			return new(ModEntry.Instance.ImproveAIcon.Sprite, Amount == -1 ? null : Amount, Colors.textMain);
		}
		return new(ModEntry.Instance.ImproveBIcon.Sprite, Amount == -1 ? null : Amount, Colors.textMain);
	}

	public override List<Tooltip> GetTooltips(State s)
	{
		if (s.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyA))
		{
			return [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve A")
				{
					Icon = ModEntry.Instance.ImproveAIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveA", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveA", "description"])
				}
			];
		}
		return [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve B")
			{
				Icon = ModEntry.Instance.ImproveBIcon.Sprite,
				TitleColor = Colors.action,
				Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveB", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveB", "description"])
			}
		];
	} 
}
