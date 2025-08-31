using System.Linq;
using Microsoft.Extensions.Logging;
using Nickel;
using Shockah.Kokoro;

namespace Flipbop.Cleo;

internal static class ImprovedAExt
{
	public static bool GetIsImprovedA(this Card self)
		=> ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(self, "ImprovedA");

	public static void SetIsImprovedA(this Card self, bool value)
		=> ModEntry.Instance.Helper.ModData.SetModData(self, "ImprovedA", value);

	public static void AddImprovedA(this Card self,  State s)
	{
		if (!self.GetIsImprovedA() && !self.GetIsImprovedB() && self.upgrade != Upgrade.A && self.upgrade != Upgrade.B && self.IsUpgradable())
		{
			SetIsImprovedA(self, true);
			ModEntry.Instance.KokoroApi.TemporaryUpgrades.SetTemporaryUpgrade(self, Upgrade.A);
			if (ModEntry.Instance.ISogginsApi is { } soggins)
			{
				if (s.EnumerateAllArtifacts().Any((a) => a is CleoSogginsArtifact))
				{
					ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, self, soggins.FrogproofTrait!, true, false);
				}
			}
		}
	}
	public static void RemoveImprovedA(this Card self, State s)
	{
		SetIsImprovedA(self, false);
		ModEntry.Instance.KokoroApi.TemporaryUpgrades.SetTemporaryUpgrade(self, null);
		ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, self, ModEntry.Instance.ImprovedATrait, false, false);
	}
}

internal sealed class ImprovedAManager
{
	internal static readonly ICardTraitEntry Trait = ModEntry.Instance.ImprovedATrait;
	
	public ImprovedAManager()
	{
		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnPlayerPlayCard),
			(State state, Card card) =>
		{
			if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(state, card, Trait) && !state.EnumerateAllArtifacts().Any((Artifact a) => a is RetainerArtifact))
			{
				if (!card.GetData(state).exhaust)
				{
					card.RemoveImprovedA(state);
					if (ModEntry.Instance.ISogginsApi is { } soggins)
					{
						if (state.EnumerateAllArtifacts().Any((a) => a is CleoSogginsArtifact))
						{
							ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(state, card, soggins.FrogproofTrait!, false, false);
						}
					}
				}
			}
		});
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatEnd), (State state) =>
		{
			foreach (var card in state.deck)
			{
				if (card.GetIsImprovedA())
				{
					card.RemoveImprovedA(state);
				}
			}
		});
	}
}
