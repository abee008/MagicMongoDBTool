﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
namespace MagicMongoDBTool.Module
{
    public class EachDatabaseUser
    {
        public Dictionary<String, MongoDBHelper.MongoUserEx> UserList = new Dictionary<string, MongoDBHelper.MongoUserEx>();
        /// <summary>
        /// 获得数据库角色
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public BsonArray GetRolesByDBName(String DatabaseName) {
            BsonArray roles = new BsonArray();
            //当前DB的System.user的角色
            if (UserList.ContainsKey(DatabaseName)) {
                roles = UserList[DatabaseName].roles;
            }
            ///Admin的OtherDBRoles和当前数据库角色合并
            BsonArray adminRoles = GetOtherDBRoles(DatabaseName);
            foreach (String item in adminRoles)
            {
                if (!roles.Contains(item)) {
                    roles.Add(item);
                }
            }
            ///ADMIN的ANY系角色的追加
            if (UserList.ContainsKey(MongoDBHelper.DATABASE_NAME_ADMIN)) { 
                if (UserList[MongoDBHelper.DATABASE_NAME_ADMIN].roles.Contains(MongoDBHelper.UserRole_dbAdminAnyDatabase)){
                    roles.Add(MongoDBHelper.UserRole_dbAdminAnyDatabase);
                }
                if (UserList[MongoDBHelper.DATABASE_NAME_ADMIN].roles.Contains(MongoDBHelper.UserRole_readAnyDatabase))
                {
                    roles.Add(MongoDBHelper.UserRole_readAnyDatabase);
                }
                if (UserList[MongoDBHelper.DATABASE_NAME_ADMIN].roles.Contains(MongoDBHelper.UserRole_readWriteAnyDatabase))
                {
                    roles.Add(MongoDBHelper.UserRole_readWriteAnyDatabase);
                }
                if (UserList[MongoDBHelper.DATABASE_NAME_ADMIN].roles.Contains(MongoDBHelper.UserRole_userAdminAnyDatabase))
                {
                    roles.Add(MongoDBHelper.UserRole_userAdminAnyDatabase);
                }
            }
            return roles;
        }
        /// <summary>
        /// 获得Admin的otherDBRoles
        /// </summary>
        /// <returns></returns>
        public BsonArray GetOtherDBRoles(String DataBaseName)
        {
            BsonArray roles = new BsonArray();
            BsonDocument OtherDBRoles = new BsonDocument();
            if (UserList.ContainsKey(MongoDBHelper.DATABASE_NAME_ADMIN)) {
                OtherDBRoles = UserList[MongoDBHelper.DATABASE_NAME_ADMIN].otherDBRoles;
                if (OtherDBRoles!=null && OtherDBRoles.Contains(DataBaseName))
                {
                    roles = OtherDBRoles[DataBaseName].AsBsonArray;
                }
            }
            return roles;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Username"></param>
        public void AddUser(MongoDatabase db,String Username){
           var userInfo = db.GetCollection(MongoDBHelper.COLLECTION_NAME_USER).FindOneAs<BsonDocument>(MongoDB.Driver.Builders.Query.EQ("user", Username));
           if (userInfo != null)
           {
               MongoDBHelper.MongoUserEx user = new MongoDBHelper.MongoUserEx();
               user.roles = userInfo["roles"].AsBsonArray;
               if (userInfo.Contains("otherDBRoles"))
               {
                   user.otherDBRoles = userInfo["otherDBRoles"].AsBsonDocument;
               }
               UserList.Add(db.Name, user); 
           }
        }
        /// <summary>
        /// 重载ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String UserInfo = String.Empty;
            foreach (var item in UserList.Keys)
            {
                UserInfo += "DataBase:" + item + System.Environment.NewLine;
                if (UserList[item].roles != null) {
                    UserInfo += "Roles:" + UserList[item].roles + System.Environment.NewLine;
                }
                if (UserList[item].otherDBRoles != null)
                {
                    UserInfo += "otherDBRoles:" + UserList[item].otherDBRoles + System.Environment.NewLine;
                }
                UserInfo += System.Environment.NewLine;
            }
            return UserInfo;
        }
    }
}
