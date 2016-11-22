using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public abstract class OnlineTexture : MonoBehaviour {
	public bool textureLoaded = false;
	protected WWW request_;


	public void Start()
	{
		// When in edit mode, start downloading a texture preview.
		if (!Application.isPlaying) {
			RequestTexturePreview ();
		}
	}


	public void RequestTexture( string nodeID )
	{
		textureLoaded = false;
		string url = GenerateRequestURL (nodeID);
		request_ = new WWW (url);
	}


	public void RequestTexturePreview ()
	{
		RequestTexture ("0");
	}


#if UNITY_EDITOR
	// Make this update with editor.
	void OnEnable(){
		EditorApplication.update += Update;
	}


	void OnDisable(){
		EditorApplication.update -= Update;
	}
#endif
		

	public void Update()
	{
		if (textureLoaded == false && request_ != null && request_.isDone) {
			string errorMessage = "";
			var tempMaterial = new Material(GetComponent<MeshRenderer> ().sharedMaterial);

			if (ValidateDownloadedTexture (out errorMessage)) {
				textureLoaded = true;
				tempMaterial.mainTexture = request_.texture;
			} else {
				request_ = null;
				tempMaterial.mainTexture = Texture2D.whiteTexture;
			}
			tempMaterial.mainTexture.wrapMode = TextureWrapMode.Clamp;
			GetComponent<MeshRenderer> ().material = tempMaterial;
		}
	}


	public bool IsDownloading()
	{
		return textureLoaded == false && request_ != null && !request_.isDone;
	}


	protected abstract string GenerateRequestURL (string nodeID);
	public void CopyTo(OnlineTexture copy)
	{
		copy.request_ = request_;
		// This forces inherited component to reload the texture.
		copy.textureLoaded = false;

		InnerCopyTo (copy);
	}

	protected abstract void InnerCopyTo (OnlineTexture copy);


	public virtual bool ValidateDownloadedTexture( out string errorMessage )
	{
		if (request_.error != null) {
			errorMessage = request_.error;
			return false;
		} else {
			errorMessage = "";
			return true;
		}
	}
}
