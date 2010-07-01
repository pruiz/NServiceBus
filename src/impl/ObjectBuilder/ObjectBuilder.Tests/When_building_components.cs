using System;
using NBehave.Spec.NUnit;
using NServiceBus.ObjectBuilder;
using NServiceBus.ObjectBuilder.Common;
using NUnit.Framework;

namespace ObjectBuilder.Tests
{
    [TestFixture]
    public class When_building_components:BuilderFixture
    {
        [Test]
        public void Singleton_components_should_yield_the_same_instance()
        {
            VerifyForAllBuilders((builder) =>
               builder.Build(typeof(SingletonComponent)).ShouldBeTheSameAs(builder.Build(typeof(SingletonComponent))));
        }

        [Test]
        public void Singlecall_components_should_yield_unique_instances()
        {
            VerifyForAllBuilders((builder) =>
               builder.Build(typeof(SinglecallComponent)).ShouldNotBeTheSameAs(builder.Build(typeof(SinglecallComponent))));
        }



        [Test]
        public void Reguesting_an_unregistered_component_should_throw()
        {
            
            VerifyForAllBuilders((builder)=> 
                Assert.That(() => builder.Build(typeof (UnregisteredComponent)),
                Throws.Exception));
        }

       
        protected override Action<IContainer> InitializeBuilder()
        {
            return (config) =>
                       {
                           config.Configure(typeof(SingletonComponent),ComponentCallModelEnum.Singleton);
                           config.Configure(typeof(SinglecallComponent), ComponentCallModelEnum.Singlecall);
                       };
        }

        public class SingletonComponent
        {
        }
        public class SinglecallComponent
        {
        }
        public class UnregisteredComponent
        {
        }
    }

   

}