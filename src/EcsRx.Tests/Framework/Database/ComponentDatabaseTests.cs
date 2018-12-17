﻿using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework.Database
{
    public class ComponentDatabaseTests
    {
        [Fact]
        public void should_correctly_initialize()
        {
            var expectedSize = 10;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            
            Assert.Equal(expectedSize, database.CurrentEntityBounds);
            Assert.Equal(fakeComponentTypes.Count, database.EntityComponents.Length);
            Assert.Equal(expectedSize, database.EntityComponents[0].Count);
            Assert.Equal(expectedSize, database.EntityComponentsLookups[0].Count);
        }
        
        [Fact]
        public void should_correctly_expand_for_new_entities()
        {
            var startingSize = 10;
            var expectedSize = 20;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, startingSize);
            database.AccommodateMoreEntities(expectedSize);
            
            Assert.Equal(expectedSize, database.CurrentEntityBounds);
            Assert.Equal(fakeComponentTypes.Count, database.EntityComponents.Length);
            Assert.Equal(expectedSize, database.EntityComponents[0].Count);
            Assert.Equal(expectedSize, database.EntityComponentsLookups[0].Count);
        }

        [Fact]
        public void should_correctly_add_component()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var fakeComponent = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, fakeComponent);
            
            Assert.Equal(database.EntityComponents[0].GetItem(1), fakeComponent);
            Assert.True(database.EntityComponentsLookups[0][1]);
            
        }
        
        [Fact]
        public void should_get_all_value_and_reference_components_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponent2 = new TestComponentThree();
            var fakeComponent3 = new TestStructComponentOne {Data = 10};
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(TestStructComponentOne), 3}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, fakeComponent1);
            database.Set(2, fakeEntityId, fakeComponent2);
            database.Set(3, fakeEntityId, fakeComponent3);

            var allComponents = database.GetAll(fakeEntityId).ToArray();
            Assert.Equal(allComponents.Length, 3);
            Assert.True(allComponents.Contains(fakeComponent1));
            Assert.True(allComponents.Contains(fakeComponent2));
            Assert.True(allComponents.Contains(fakeComponent3));
        }
        
        [Fact]
        public void should_only_get_components_for_single_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponent2 = new TestComponentThree();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, fakeComponent1);
            database.Set(2, fakeEntityId, fakeComponent2);
            database.Set(0, otherEntityId, new TestComponentOne());
            database.Set(1, otherEntityId, new TestComponentTwo());
            database.Set(2, otherEntityId, new TestComponentThree());

            var allComponents = database.GetAll(fakeEntityId).ToArray();
            Assert.Equal(allComponents.Length, 2);
            Assert.True(allComponents.Contains(fakeComponent1));
            Assert.True(allComponents.Contains(fakeComponent2));
        }
        
        [Fact]
        public void should_get_specific_components_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, fakeComponent1);
            database.Set(0, otherEntityId, new TestComponentOne());

            var component = database.Get<TestComponentOne>(0, fakeEntityId);
            Assert.Equal(fakeComponent1, component);
        }
        
        [Fact]
        public void should_correctly_identify_if_component_exists_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, fakeComponent1);
            database.Set(0, otherEntityId, new TestComponentOne());
            database.Set(1, otherEntityId, new TestComponentTwo());

            var hasComponent0 = database.Has(0, fakeEntityId);
            Assert.True(hasComponent0);
            
            var hasComponent1 = database.Has(1, fakeEntityId);
            Assert.False(hasComponent1);
        }
        
        [Fact]
        public void should_correctly_remove_component_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, fakeComponent1);
            database.Set(0, otherEntityId, new TestComponentOne());
            database.Set(1, otherEntityId, new TestComponentTwo());

            database.Remove(0, fakeEntityId);
            Assert.False(database.Has(0, fakeEntityId));
        }
        
        [Fact]
        public void should_correctly_remove_all_components_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, new TestComponentOne());
            database.Set(1, fakeEntityId, new TestComponentTwo());
            database.Set(2, fakeEntityId, new TestComponentThree());
            database.Set(0, otherEntityId, new TestComponentOne());
            database.Set(1, otherEntityId, new TestComponentTwo());

            database.RemoveAll(fakeEntityId);
            Assert.False(database.Has(0, fakeEntityId));
            Assert.False(database.Has(1, fakeEntityId));
            Assert.False(database.Has(2, fakeEntityId));

            var allComponents = database.GetAll(fakeEntityId);
            Assert.Empty(allComponents);
        }
        
        [Fact]
        public void should_get_collection_for_components()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestStructComponentOne), 1}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Set(0, fakeEntityId, new TestComponentOne());
            database.Set(0, otherEntityId, new TestComponentOne());
            database.Set(1, fakeEntityId, new TestStructComponentOne());
            database.Set(1, otherEntityId, new TestStructComponentOne());

            var refComponents = database.GetComponents<TestComponentOne>(0);
            var structComponents = database.GetComponents<TestStructComponentOne>(1);

            Assert.Equal(2, refComponents.Count(x => x != null));
            Assert.Equal(10, structComponents.Count);
        }
    }
}