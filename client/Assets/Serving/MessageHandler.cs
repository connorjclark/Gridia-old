using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serving
{
    public abstract class MessageHandler<S, T>
        where S : SocketHandler
    {
        private static Dictionary<String, Type> _handlersBySimpleName;

        public static Type Get(String simpleName) {
            if (_handlersBySimpleName == null)
            {
                LoadMessageHandlers();
            }
            return _handlersBySimpleName[simpleName];
        }

        // :(
        private static void LoadMessageHandlers()
        {
            _handlersBySimpleName = new Dictionary<String, Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var subtypes = assembly.GetTypes().Where(type =>
                    ReflexionExtension.IsSubClassOfGeneric(type, typeof(MessageHandler<SocketHandlerImpl, Object>))
                );
                foreach (var type in subtypes)
                {
                    _handlersBySimpleName[(String)type.Name] = type;
                }
            }
        }

        protected abstract T Interpret(byte[] bytes);

        protected abstract void Handle(S connection, T data);

        public void Handle(S connection, byte[] bytes) {
            Handle(connection, Interpret(bytes));
        }
    }
}
