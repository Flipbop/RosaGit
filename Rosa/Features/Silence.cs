using System.Collections.Generic;
using HarmonyLib;
using Nickel;
using System.Reflection;
using FSPRO;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class SilenceManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    internal static IStatusEntry SilenceStatus { get; private set; } = null!;
    public SilenceManager()
    {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(new StatusLogicHook(), 0);
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActionsOverridden)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_GetActionsOverridden_Postfix)))
            );
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AStatus_Begin_Postfix)))
            );
        
    }
    private static void Card_GetActionsOverridden_Postfix(State s, ref List<CardAction> __result, Card __instance)
    {
        var card = __instance;
        if (card is null) return;
        if (s.route is not Combat c) return;
        if (s.ship.Get(SilenceStatus.Status) <= 0) return;
        if (s.FindCard(card.uuid) is null) return;
        foreach (var actions in __result)
        {
            foreach (var wrappedAction in ModEntry.Instance.KokoroApi.WrappedActions.GetWrappedCardActionsRecursively(actions))
            {
                if (wrappedAction is not AAttack)
                {
                    wrappedAction.disabled = true;
                }
            }
        }
    }
    private static void AStatus_Begin_Postfix(G g, State s, Combat c, AStatus __instance)
    {
        if (__instance.status == SilenceStatus.Status)
        {
            if (__instance.targetPlayer == false)
            {
                if (c.otherShip.Get(SilenceStatus.Status) < 1) return;
                foreach (var part in c.otherShip.parts)
                {
                    if (part is null) continue;
                    if (part.intent is not IntentAttack)
                    {
                        part.intent = null;
                    }
                }
                Audio.Play(Event.Status_ShieldDown);
            }
        }
    }
    private sealed class StatusLogicHook : IKokoroApi.IV2.IStatusLogicApi.IHook
    {
        public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
        {
            if (args.Status == SilenceStatus.Status)
            {
                if (args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd)
                {
                    if (args.Amount > 0)
                    {
                        args.Amount -= 1;
                        return true;
                    }
                }
                else
                {
                    if (args.Combat.otherShip.Get(SilenceStatus.Status) > 2)
                    {
                        foreach (var part in args.Combat.otherShip.parts)
                        {
                            if (part is null) continue;
                            if (part.intent is not IntentAttack)
                            {
                                part.intent = null;
                            }
                        }
                        Audio.Play(Event.Status_ShieldDown);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
