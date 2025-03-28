namespace GoodLuckValley.Events.Cinematics
{
    public struct PlayTimeline : IEvent 
    {
        public int ID;
    }

    public struct StartCinematic : IEvent { }

    public struct EndCinematic : IEvent { }

    public struct CutToBlack : IEvent { }
}
