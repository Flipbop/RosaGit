using Nickel;
using System.Linq;

namespace Flipbop.Rosa;

internal static class ImprovedBExt
{
	public static bool GetIsImprovedB(this Card self)
		=> ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(self, "ImprovedB");

	public static void SetIsImprovedB(this Card self, bool value)
		=> ModEntry.Instance.Helper.ModData.SetModData(self, "ImprovedB", value);

	public static void AddImprovedB(this Card self, State s)
	{
		if (!self.GetIsImprovedA() && !self.GetIsImprovedB() && self.upgrade != Upgrade.A && self.upgrade != Upgrade.B && self.IsUpgradable())
		{
			SetIsImprovedB(self, true);
			ModEntry.Instance.KokoroApi.TemporaryUpgrades.SetTemporaryUpgrade(self, Upgrade.B);
			
		}
	}
	public static void RemoveImprovedB(this Card self, State s)
	{
		SetIsImprovedB(self, false);
		ModEntry.Instance.KokoroApi.TemporaryUpgrades.SetTemporaryUpgrade(self, null);
		ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(s, self, ModEntry.Instance.ImprovedBTrait, false, false);
	}
}

internal sealed class ImprovedBManager
{
	internal static readonly ICardTraitEntry Trait = ModEntry.Instance.ImprovedBTrait;

	public ImprovedBManager()
	{
		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnPlayerPlayCard), (State state, Card card) =>
		{
			if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(state, card, Trait) && !state.EnumerateAllArtifacts().Any((Artifact a) => a is RetainerArtifact))
			{
				if (!card.GetData(state).exhaust)
				{
					card.RemoveImprovedB(state);
					
				}
			}
		});
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatEnd), (State state) =>
		{
			foreach (var card in state.deck)
			{
				if (card.GetIsImprovedB())
				{
					card.RemoveImprovedB(state);
				}
			}
		});
	}
}
