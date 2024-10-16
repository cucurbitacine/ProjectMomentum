namespace Game.Scripts.Core
{
    public interface IMass
    {
        public const float Mass2Rigid = 0.01f;
        
        public float GetMass();
    }
}