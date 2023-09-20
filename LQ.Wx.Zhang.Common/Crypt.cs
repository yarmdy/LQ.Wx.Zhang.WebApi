using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using System.Text;

namespace AiBi.Test.Common
{
    /// <summary>
    /// 加密解密帮助类
    /// </summary>
    public class Crypt
    {
        public int RsaKeySize { get; set; } = 512;
        public string RsaPrivateKey { get; set; } = "<RSAKeyValue><Modulus>yCKCOoRNnJ78SrrCpGAUf+RPXIonWDFg1txjY9e943fsY8YiIvBM1+fA4X5KUXybHTNFIapEyYRbANLvw3dUnQ==</Modulus><Exponent>AQAB</Exponent><P>88pPE0T741VN2MtWoa9ehH0YI339BxYj//WPKkXEI2s=</P><Q>0ih7phJnYMxyiRIzm7GOUl/4NsbbZhkuOoVA5yRX8hc=</Q><DP>RCUFy34Z5qa+lt3nvlQ12FTbPXiHFMcEkxSByzArLjc=</DP><DQ>y2RYvxKCsKFNnDFcdytxZXHyJPlyLy7hmcuLU+jNoMM=</DQ><InverseQ>8JZr+RbCCSYZnbfuErpBTuH7cHgQVp2Z8Yrz7lreBBA=</InverseQ><D>ni0QClnI0ZGiSxnifnHlodL7mmMih4S3Sfnzn9TCosUdyswZXk+AZB8n7IL1uyeVby9pus8q/RbDN8eKnssMTQ==</D></RSAKeyValue>";
        public string RsaPublicKey { get; set; } = "<RSAKeyValue><Modulus>yCKCOoRNnJ78SrrCpGAUf+RPXIonWDFg1txjY9e943fsY8YiIvBM1+fA4X5KUXybHTNFIapEyYRbANLvw3dUnQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        static string md5Key
        {
            get
            {
                return "31a90d4e82b6f5c7";
            }
        }

        static string aesKey
        {
            get
            {
                return "AIIBIIAIIBII2020";
            }
        }
        static string aesIV
        {
            get
            {
                return "aiibiiaiibii2020";
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string? RSADecrypt(string str)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    rsa.FromXmlString(RsaPrivateKey);
                    var cryptData = Convert.FromBase64String(str);
                    var strData = rsa.Decrypt(cryptData,false);
                    return Encoding.UTF8.GetString(strData);
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string RSAEncrypt(string str)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                rsa.FromXmlString(RsaPublicKey);
                var strData = Encoding.UTF8.GetBytes(str);
                var cryptData = rsa.Encrypt(strData,false);
                return Convert.ToBase64String(cryptData);
            }
        }

        public static string Md5(string strText, string? strKey = null)
        {
            string strResult = "";
            var source = Encoding.UTF8.GetBytes(strText);
            var keys = strKey != null ? strKey.ToCharArray() : md5Key.ToCharArray();	// 加密密钥

            //var MD5CSP = new MD5CryptoServiceProvider();
            var MD5CSP = MD5.Create();
            var tmp = MD5CSP.ComputeHash(source);    // MD5的计算结果是一个128位的长整数，用字节表示就是16个字节
            var str = new char[16 * 2]; 		        // 每个字节用16进制表示的话使用两个字符，所以表示成16进制需要32个字符

            int k = 0; 	                                // 表示转换结果中对应的字符位置
            for (int i = 0; i < 16; i++)
            {
                // 从第一个字节开始，对 MD5 的每一个字节转换成16进制字符的转换
                byte byte0 = tmp[i]; 					// 取第i个字节
                str[k++] = keys[byte0 >> 4 & 0xf]; 	    // 取字节中高4位的数字转换,>>> 为补零右移(即无符号)，将符号位一起右移
                str[k++] = keys[byte0 & 0xf]; 			// 取字节中低4位的数字转换
            }
            strResult = new string(str);                // 换后的结果转换为字符串

            return strResult;
        }

        public static string? AesEncrypt(string str, string? key = null, string? iv = null)
        {
            try
            {
                var strData = Encoding.UTF8.GetBytes(str);

                if (key == null)
                {
                    key = aesKey;
                }
                if (iv == null)
                {
                    iv = aesIV;
                }
                var keyData = Encoding.ASCII.GetBytes(key);
                var ivData = Encoding.ASCII.GetBytes(iv);

                byte[]? res = null;

                if (keyData.Length != 16)
                {
                    keyData = keyData.Take(16).ToArray();
                    keyData = keyData.Concat(new byte[16 - keyData.Length]).ToArray();
                }
                if (ivData.Length != 16)
                {
                    ivData = ivData.Take(16).ToArray();
                    ivData = ivData.Concat(new byte[16 - ivData.Length]).ToArray();
                }
                //using (var aesProvider = new AesCryptoServiceProvider { Key = keyData, IV = ivData, KeySize = 128, BlockSize = 128, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 })
                using (var aesProvider = Aes.Create())
                {
                    aesProvider.Key=keyData; aesProvider.IV=ivData;aesProvider.KeySize = 128;aesProvider.BlockSize = 128;aesProvider.Mode = CipherMode.CBC;aesProvider.Padding = PaddingMode.PKCS7;
                    using (var trans = aesProvider.CreateEncryptor(keyData, ivData))
                    {
                        res = trans.TransformFinalBlock(strData, 0, strData.Length);
                    }
                }
                return Convert.ToBase64String(res);
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static string? AesDecrypt(string str, string? key = null, string? iv = null)
        {
            try
            {
                var strData = Convert.FromBase64String(str);

                if (key == null)
                {
                    key = aesKey;
                }
                if (iv == null)
                {
                    iv = aesIV;
                }
                var keyData = Encoding.ASCII.GetBytes(key);
                var ivData = Encoding.ASCII.GetBytes(iv);

                byte[]? res = null;

                if (keyData.Length != 16)
                {
                    keyData = keyData.Take(16).ToArray();
                    keyData = keyData.Concat(new byte[16 - keyData.Length]).ToArray();
                }
                if (ivData.Length != 16)
                {
                    ivData = ivData.Take(16).ToArray();
                    ivData = ivData.Concat(new byte[16 - ivData.Length]).ToArray();
                }
                //using (var aesProvider = new AesCryptoServiceProvider { Key = keyData, IV = ivData, KeySize = 128, BlockSize = 128, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 })
                using (var aesProvider = Aes.Create())
                {
                    aesProvider.Key = keyData; aesProvider.IV=ivData;aesProvider.KeySize = 128;aesProvider.BlockSize = 128;aesProvider.Mode = CipherMode.CBC;aesProvider.Padding = PaddingMode.PKCS7;
                    using (var trans = aesProvider.CreateDecryptor(keyData, ivData))
                    {
                        res = trans.TransformFinalBlock(strData, 0, strData.Length);
                    }
                }
                return Encoding.UTF8.GetString(res);
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
