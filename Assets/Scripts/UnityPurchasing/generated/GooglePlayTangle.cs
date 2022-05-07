// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static readonly byte[] data = System.Convert.FromBase64String("verT2YIyWt1+xcdDFoQ6NvfJ5svqWNv46tfc0/Bcklwt19vb29/a2SrfGdLJ37CX+xrUE+tIv7Ge1WEeNG5jqPKlnqKhI+2s4D4cpkLk74u3F6QzP07/pN2FVHVGZCHINW3Iowg5PwIB4ag7cumdVWeOmL/93L7smovC81440+bNFVQJ56rmHlAjU1kU3uw+/y9M5olkb4ywWpjs7WHW7GySB0yYBQZmxWqOESomZDibPBnDQkndtaolB92Rxki4iFdIIYNTlCm/9MahWdgfmnUU1ZQvo0M7b19L9VtufJxkUyZ+umwC+/4nszOHMb++WNvV2upY29DYWNvb2nvOv5+b01t3UkVLjAQi832xEUaCFMAB+zM6MZnKuL3hEb/J+djZ29rb");
        private static readonly int[] order = new int[] { 11,11,9,9,8,5,8,9,9,9,10,13,13,13,14 };
        private static readonly int key = 218;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
