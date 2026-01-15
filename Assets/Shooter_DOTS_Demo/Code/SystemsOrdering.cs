/*namespace Code
{
    using Unity.Entities;

    [UpdateInGroup(typeof(MyCustomGroup))]
    public partial struct InitSystem : ISystem { }

    [UpdateInGroup(typeof(MyCustomGroup))]
    [UpdateAfter(typeof(InitSystem))]
    public partial struct ProcessSystem : ISystem { }

    [UpdateInGroup(typeof(MyCustomGroup))]
    [UpdateAfter(typeof(ProcessSystem))]
    public partial struct RenderPrepareSystem : ISystem { }
}*/