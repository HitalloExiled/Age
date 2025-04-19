using Age.Elements;

namespace Age.Tests.Age.Elements;

public partial class ComposedTreeTraversalEnumeratorTest
{
    public class HostElement : Element
    {
        public override string NodeName => nameof(HostElement);

        public Element[] ShadowNodes { get; }

        public HostElement(string name)
        {
            this.AttachShadowTree();

            this.Name = name;

            TestElement       child1;
            TestElement       child21;
            TestElement       child22;
            Slot              child22Slot;
            TestElement       child23;
            TestElement       child2;
            TestElement       child31;
            Slot              child31Slot;
            TestElement       child31Slot1;
            TestElement       child32;
            TestElement       child3;
            NestedHostElement child4;
            TestElement       child5;
            TestElement       child51;
            Slot              child6;

            this.ShadowTree.Children =
            [
                child1 = new TestElement($"{name}.#.1"),
                child2 = new TestElement($"{name}.#.2")
                {
                    Children =
                    [
                        child21 = new TestElement($"{name}.#.2.1"),
                        child22 = new TestElement($"{name}.#.2.2")
                        {
                            Children =
                            [
                                child22Slot = new Slot
                                {
                                    Name     = $"{name}.#.2.2.(1)",
                                    Children =
                                    [
                                        new TestElement("ignored"),
                                    ],
                                }
                            ],
                        },
                        child23 = new TestElement($"{name}.#.2.3"),
                    ],
                },
                child3 = new TestElement($"{name}.#.3")
                {
                    Children =
                    [
                        child31 = new TestElement($"{name}.#.3.1")
                        {
                            Children =
                            [
                                child31Slot = new Slot
                                {
                                    Name     = $"{name}.#.3.1.(1)",
                                    Children =
                                    [
                                        child31Slot1 = new TestElement($"{name}.#.3.1.(1).1"),
                                    ]
                                },
                            ],
                        },
                        child32 = new TestElement($"{name}.#.3.2"),
                    ],
                },
                child4 = new NestedHostElement($"{name}.#.4"),
                child5 = new TestElement($"{name}.#.5")
                {
                    Children =
                    [
                        child51 = new TestElement($"{name}.#.5.1"),
                    ],
                },
                child6 = new Slot(),
            ];

            this.ShadowNodes =
            [
                child1,
                child2,
                child21,
                child22,
                child22Slot,
                child23,
                child3,
                child31,
                child31Slot,
                child31Slot1,
                child32,
                child4,
                child5,
                child51,
                child6,
            ];
        }
    }
}
