namespace View.Battle
{
    public class BossWeaknessPointView : BaseView
    {
        public int OwnerUnitId { get; private set; }

        public void Connect(int ownerUnitId)
        {
            OwnerUnitId = ownerUnitId;
        }
    }
}