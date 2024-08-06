using MemoryPack;

namespace ET

{
    [ComponentOf(typeof(LSWorld))]
    [MemoryPackable]
    public partial class LSBulletComponent:LSEntity, IAwake,ILSUpdate, ISerializeToEntity
    {
        public int _autoId;
    }
}