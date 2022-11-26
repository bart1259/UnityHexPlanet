using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctTree <K>
{
    public const int MAX_DEPTH = 5;
    public const int MAX_POINTS_PER_LEAF = 6;

    private Vector3 _backBottomLeft;
    private Vector3 _frontUpperRight;
    private Vector3 _center;
    private Vector3 _size;
    private Dictionary<K, Vector3> _points;
    private int _depth = 0;
    private bool _split = false;

    private OctTree<K> _bblOctTree = null;
    private OctTree<K> _fblOctTree = null;
    private OctTree<K> _btlOctTree = null;
    private OctTree<K> _ftlOctTree = null;
    private OctTree<K> _bbrOctTree = null;
    private OctTree<K> _fbrOctTree = null;
    private OctTree<K> _btrOctTree = null;
    private OctTree<K> _ftrOctTree = null;

    public OctTree(Vector3 backBottomLeft, Vector3 frontUpperRight, int depth) : this(backBottomLeft, frontUpperRight) {
        this._depth = depth;
    }

    public OctTree(Vector3 backBottomLeft, Vector3 frontUpperRight) {
        _frontUpperRight = frontUpperRight;
        _backBottomLeft = backBottomLeft;
        _center = (frontUpperRight + backBottomLeft) / 2.0f;
        _size = frontUpperRight - backBottomLeft;

        _points = new Dictionary<K, Vector3>();
    }

    private void Split() {
        _split = true;

        _bblOctTree = new OctTree<K>(_backBottomLeft, _center, _depth + 1);
        _fblOctTree = new OctTree<K>(_backBottomLeft + (Vector3.forward * (_size.z / 2)), _center + (Vector3.forward * (_size.z / 2)), _depth + 1);
        _btlOctTree = new OctTree<K>(_backBottomLeft + (Vector3.up * (_size.y / 2)), _center + (Vector3.up * (_size.y / 2)), _depth + 1);
        _ftlOctTree = new OctTree<K>(_backBottomLeft + (Vector3.up * (_size.y / 2)) + (Vector3.forward * (_size.z / 2)), _center + (Vector3.up * (_size.y / 2)) + (Vector3.forward * (_size.z / 2)), _depth + 1);
        _bbrOctTree = new OctTree<K>(_backBottomLeft + (Vector3.right * (_size.x / 2)), _center + (Vector3.right * (_size.x / 2)), _depth + 1);
        _fbrOctTree = new OctTree<K>(_backBottomLeft + (Vector3.forward * (_size.z / 2)) + (Vector3.right * (_size.x / 2)), _center + (Vector3.forward * (_size.z / 2)) + (Vector3.right * (_size.x / 2)), _depth + 1);
        _btrOctTree = new OctTree<K>(_backBottomLeft + (Vector3.up * (_size.y / 2)) + (Vector3.right * (_size.x / 2)), _center + (Vector3.up * (_size.y / 2)) + (Vector3.right * (_size.x / 2)), _depth + 1);
        _ftrOctTree = new OctTree<K>(_backBottomLeft + (Vector3.up * (_size.y / 2)) + (Vector3.forward * (_size.z / 2)) + (Vector3.right * (_size.x / 2)), _center + (Vector3.up * (_size.y / 2)) + (Vector3.forward * (_size.z / 2)) + (Vector3.right * (_size.x / 2)), _depth + 1);

        foreach (KeyValuePair<K, Vector3> kvp in _points)
        {
            InsertPointInternally(kvp.Key, kvp.Value);
        }
        _points.Clear();
    }

    private void InsertPointInternally(K key, Vector3 pos) {
        if(pos.x > _center.x) {
            // Right
            if(pos.y > _center.y) {
                // Top
                if(pos.z > _center.z) {
                    // Front
                    _ftrOctTree.InsertPoint(key, pos);
                } else {
                    // Back
                    _btrOctTree.InsertPoint(key, pos);
                }
            } else {
                // Bottom
                if(pos.z > _center.z) {
                    // Front
                    _fbrOctTree.InsertPoint(key, pos);
                } else {
                    // Back
                    _bbrOctTree.InsertPoint(key, pos);
                }
            }
        } else {
            // Left
            if(pos.y > _center.y) {
                // Top
                if(pos.z > _center.z) {
                    // Front
                    _ftlOctTree.InsertPoint(key, pos);
                } else {
                    // Back
                    _btlOctTree.InsertPoint(key, pos);
                }
            } else {
                // Bottom
                if(pos.z > _center.z) {
                    // Front
                    _fblOctTree.InsertPoint(key, pos);
                } else {
                    // Back
                    _bblOctTree.InsertPoint(key, pos);
                }
            }
        }
    }

    public void InsertPoint(K key, Vector3 pos) {
        if(!_split && _points.Count < MAX_POINTS_PER_LEAF) {
            _points.Add(key, pos);
            return;
        }

        if(!_split && _depth >= MAX_DEPTH) {
            // Can't split anymore, adding to list
            _points.Add(key, pos);
            return;
        }

        // Split
        if(!_split) {
            Split();
        }

        InsertPointInternally(key, pos);
    }

    public List<K> GetPoints(Vector3 center, Vector3 size) {
        List<K> ret = new List<K>();
        
        if(!BoxIntersectsBox(center, size, _center, _size)) {
            return ret;
        }

        if(!_split) {
            foreach (KeyValuePair<K, Vector3> kvp in _points)
            {
                if(PointWithinBox(center, size, kvp.Value)) {
                    ret.Add(kvp.Key);
                }
            }
            return ret;
        }

        ret.AddRange(_bblOctTree.GetPoints(center, size));
        ret.AddRange(_fblOctTree.GetPoints(center, size));
        ret.AddRange(_btlOctTree.GetPoints(center, size));
        ret.AddRange(_ftlOctTree.GetPoints(center, size));
        ret.AddRange(_bbrOctTree.GetPoints(center, size));
        ret.AddRange(_fbrOctTree.GetPoints(center, size));
        ret.AddRange(_btrOctTree.GetPoints(center, size));
        ret.AddRange(_ftrOctTree.GetPoints(center, size));

        return ret;
    }

    private bool BoxIntersectsBox(Vector3 boxACenter, Vector3 boxASize, Vector3 boxBCenter, Vector3 boxBSize) {
        return ((boxACenter.x - boxASize.x <= boxBCenter.x + boxBSize.x) && (boxACenter.x + boxASize.x >= boxBCenter.x - boxBSize.x)) &&
               ((boxACenter.y - boxASize.y <= boxBCenter.y + boxBSize.y) && (boxACenter.y + boxASize.y >= boxBCenter.y - boxBSize.y)) &&
               ((boxACenter.z - boxASize.z <= boxBCenter.z + boxBSize.z) && (boxACenter.z + boxASize.z >= boxBCenter.z - boxBSize.z));
    }

    private bool PointWithinBox(Vector3 boxCenter, Vector3 boxSize, Vector3 point) {
        return (point.x <= boxCenter.x + boxSize.x) && (point.x >= boxCenter.x - boxSize.x) &&
               (point.y <= boxCenter.y + boxSize.y) && (point.y >= boxCenter.y - boxSize.y) &&
               (point.z <= boxCenter.z + boxSize.z) && (point.z >= boxCenter.z - boxSize.z);
    }

}