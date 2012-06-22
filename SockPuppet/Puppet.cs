using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using System.Web.Script.Serialization;

namespace SockPuppet
{
    public class Puppet : ImpromptuObject
    {
        private Action<string> Send;

        public Puppet(Action<string> send)
        {
            this.Send = send;
        }

        private string subContext = String.Empty;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            this.subContext += "." + binder.Name;

            result = this;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            string fullMethodName = (this.subContext + "." + binder.Name).TrimStart('.');

            string payload = CreateJsonMethodCall(fullMethodName, args);

            this.Send(payload);

            this.subContext = String.Empty;

            result = null;
            return true;
        }

        private static string CreateJsonMethodCall(string name, IEnumerable<object> arguments)
        {
            var methodCall = new
            {
                SockPuppetCall = new
                {
                    Name = name,
                    Arguments = arguments
                }
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(methodCall);
        }

        public static dynamic New(Action<string> send)
        {
            dynamic puppet = new Puppet(send);

            return puppet;
        }

        public static T New<T>(Action<string> send) where T : class
        {
            dynamic puppet = new Puppet(send);

            return ImpromptuInterface.Impromptu.ActLike<T>(puppet);
        }
    }
}
