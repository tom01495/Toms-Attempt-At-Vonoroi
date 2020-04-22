using System;
using System.Collections.Generic;
using System.Linq;
using FortunesAlgoritmGeometry;
using UnityEngine;

//#nullable enable // Unity doesnt like this

public class FortunesAlgorithm {
    private List<Boundary> V;
    private List<Site> S;

    public FortunesAlgorithm(List<Vector2> siteCoordinates) {
        V = new List<Boundary>();
        S = siteCoordinates.ConvertAll<Site>(p => new Site(p.x, p.y));
        S.Sort();

        FortunesPlaneSweepingAlgorithm();

        //VonoroiDebugger.ShowBoundaries(V, float.MaxValue); // TODO delete this
    }

    private void FortunesPlaneSweepingAlgorithm() {
        List<Site> initialSites = InitialSites(S);
        List<Point> Q = InitialQ(initialSites, S);
        List<PointSet> T = InitialT(initialSites);

        while(Q.Any()) { 
            Point p = DeleteMax(Q);

            if(typeof(Site).IsInstanceOfType(p)) { SiteEvent((Site)p, Q, T); }
            else if(typeof(Vertex).IsInstanceOfType(p)) { CircleEvent((Vertex)p, Q, T); }
        }

        foreach(Boundary C in T.OfType<Boundary>()) { AddIfNotInList(V, C.Normal()); }
    }

    // ================================== Initiating Functions

    private static List<Site> InitialSites(List<Site> S) {
        return S.Where(p => p.CompareTo(S.Max()) == 0).ToList();
    }

    private static List<Point> InitialQ(List<Site> initialSites, List<Site> S) {
        return (S.Except(initialSites)).Cast<Point>().ToList();
    }

    private static List<PointSet> InitialT(List<Site> initialSites) {
        List<PointSet> T = new List<PointSet>();
        T.Add(new Region(initialSites[0]));

        for(int i = 1; i < initialSites.Count; i++) {
            T.Add(new Boundary(initialSites[i-1],initialSites[i]).Zero());
            T.Add(new Region(initialSites[i]));
        }

        return T;
    }

    // ================================== Site Event

    private void SiteEvent(Site p, List<Point> Q, List<PointSet> T) {
        // Step 1: Finding the region above site p
        Region Rq = Region.InRegion(T.OfType<Region>().ToList(), p);
        Site q = Rq.Site;
        Boundary Crq = null; //Boundary?
        Boundary Cqs = null; //Boundary?

        int indexRq = T.FindIndex(r => Rq.Equals(r));
        if(indexRq > 0) { Crq = T[indexRq-1] as Boundary; }
        if(indexRq < T.Count - 1) { Cqs = T[indexRq+1] as Boundary; }

        // Step 2: Creating a new region and inserting it in the beachline under the region above
        Region Rp = new Region(p);
        Boundary Cpq = new Boundary(p, q);

        T.RemoveAt(indexRq);
        List<PointSet> additionT = new List<PointSet>() {Rq.Left(Rp), Cpq.Neg(), Rp, Cpq.Pos(), Rq.Right(Rp)};
        T.InsertRange(indexRq, additionT);

        // Step 3: Adding and removing (new) intersections in Q
        if(Crq != null && Cqs != null) { Q.RemoveAll(point => Boundary.IsIntersection(Crq, Cqs, point)); }
        if(Crq != null) { AddIfNotNull(Q, Crq.Intersection(Cpq.Neg())); }
        if(Cqs != null) { AddIfNotNull(Q, Cpq.Pos().Intersection(Cqs)); }
    }

    // ================================== Circle Event

    private void CircleEvent(Vertex p, List<Point> Q, List<PointSet> T) {
        // Step 1: Finding the intersection of the two boundaries of the intersection
        Boundary Cqr = p.LeftBoundary;
        Boundary Crs = p.RightBoundary;
        Site q = Cqr.LeftSite;
        Site s = Crs.RightSite;
        Boundary Cuq = null; //Boundary?
        Boundary Csv = null; //Boundary?

        int indexCqr = IndexOfBoundary(T, Cqr);
        if(indexCqr > 1) { Cuq = T[indexCqr-2] as Boundary; }
        if(indexCqr < T.Count - 4) { Csv = T[indexCqr+4] as Boundary; }

        // Step 2: Creating a new boundary and cover up in the beachline the boundaries and region from which it starts
        Boundary Cqs = Boundary.CreateSubtype(q, s);

        T.RemoveRange(indexCqr, 3);
        T.Insert(indexCqr, Cqs);

        CloseVertex(p, Cqr, Crs, Cqs);

        // Step 3: Adding and removing (new) intersections in Q
        if(Cuq != null) { Q.RemoveAll(point => Boundary.IsIntersection(Cuq, Cqr, point)); }
        if(Csv != null) { Q.RemoveAll(point => Boundary.IsIntersection(Crs, Csv, point)); }
        if(Cuq != null) { AddIfNotNull(Q, Cuq.Intersection(Cqs)); } // TODO check adding intersection again
        if(Csv != null) { AddIfNotNull(Q, Cqs.Intersection(Csv)); }
    }

    private static int IndexOfBoundary(List<PointSet> T, Boundary C) {
        for(int index = 0; index < T.Count; index++) {
            Boundary elem = T[index] as Boundary;
            if(elem != null && C.Equals(elem)) { return index; }
        }
        throw new Exception("Border not found!");
    }

    private void CloseVertex(Vertex p, Boundary Cqr, Boundary Crs, Boundary Cqs) {
        Cqr.Summit = p;
        Crs.Summit = p;
        Cqs.Base = p;
        p.OutgoingBoundary = Cqs;

        AddIfNotInList(V, Cqr.Normal());
        AddIfNotInList(V, Crs.Normal());
    }

    // ================================== Additional Functions (as instructed)

    private static Point DeleteMax(List<Point> Q) {
        Point p = Q.Max();
        Q.Remove(p);
        return p;
    }

    private static void AddIfNotNull<T>(List<T> L, T e) { //T?
        if(e != null) L.Add(e);
    }

    private static void AddIfNotInList<T>(List<T> L, T e) {
        if(!L.Contains(e)) L.Add(e);
    }

    // ==================================== Helper Functions (use these before you want result)

    public FortunesAlgorithm RemoveShortBoundaries(float minLenght) { // TODO not urgent
        foreach(Boundary C in V) {
            //if(C.base_.Dist(C.summit) < minLenght) {
                //V.Remove(C);
            //}
        }
        return this;
    }

    public FortunesAlgorithm MakeBoundariesWiggely() {
        // Minkowski Distance
        return this;
    }

    public FortunesAlgorithm CutCorners(Rect bounds) { //This algorithm doesnt work i think. I think i need to do it in the algorithm itself
        // Creates 4 additional semi boundaries, like a box
        // tries to find the closest intersections with boundaries without a summit/base
        // for each of these intersections cuts the semi boundaries in short boundaries on the side
        // make the neighbouring region both ends of the boundary (to prevent messing up checks later)
        // add these side boundaries to the list of boundaries
        return this;
    }

    // ==================================== Get Function

    public (List<BorderInit>, List<TileInit>) GetBordersAndTiles() {
        List<BorderInit> borders = new List<BorderInit>();
        List<TileInit> tiles = new List<TileInit>();

        Dictionary<int, BorderInit> borderDict = new Dictionary<int, BorderInit>();
        Dictionary<int, TileInit> tileDict = new Dictionary<int, TileInit>();

        // Step 1: Setting the borders & tiles
        foreach(Boundary C in V) {
            BorderInit border = C.CreateUnsetBorder();
            if(border != null) {
                if(!borderDict.ContainsKey(border.GetHashCode())) {
                    borders.Add(border);
                    borderDict.Add(border.GetHashCode(), border);
                }
            }
        }
        foreach(Site s in S) {
            TileInit tile = s.CreateUnsetTile();
            if(tile != null) {
                if(!tileDict.ContainsKey(tile.GetHashCode())) {
                    tiles.Add(tile);
                    tileDict.Add(tile.GetHashCode(), tile);
                }
            }
        }

        // Step 2: Correcting the refrences
        foreach(UnsetBorderInit b in borders) {
            TileInit leftTile = SearchDict(tileDict, b.LeftSite);
            TileInit rightTile = SearchDict(tileDict, b.RightSite);
            List<BorderInit> borderBase = SearchDictList(borderDict, b.BaseNeighbours);
            List<BorderInit> borderSummit = SearchDictList(borderDict, b.SummitNeighbours);
            b.SetBorderInit(leftTile, rightTile, borderBase, borderSummit);
        }
        foreach(UnsetTileInit t in tiles) {
            List<BorderInit> tileBorders = SearchDictList(borderDict, t.Boundaries);
            t.SetTileInit(tileBorders);
        }

        return (borders, tiles);
    }

    private List<T1> SearchDictList<T1, T2>(Dictionary<int, T1> dict, List<T2> list) {
        List<T1> output = new List<T1>();
        foreach(T2 e in list) output.Add(SearchDict(dict, e));
        return output;
    }

    private T1 SearchDict<T1, T2>(Dictionary<int, T1> dict, T2 e) {
        T1 output;
        if(dict.TryGetValue(e.GetHashCode(), out output)) return output;
        else return default(T1);
    }
}

// PSEUDO CODE

// let ∗(z) be the transformation ∗(z) = (z.x, z.y + d(z)) where d(z) is the Euclidean distance between z and the nearest site
// let T be the "beach line"
// let R(p) be the region covered by site p.
// let C(p,q) be the boundary ray between sites p and q.
// let p1,p2,...,pm be the sites with minimal y-coordinate, ordered by x-coordinate
// Q ← S - {p1,p2,...,pm}
// create initial vertical boundary rays C(0)(p1,p2), C(0)(p2,p3),...C(0)(pm−1,pm)
// T ← ∗(R(p1)), C(0)(p1,p2), ∗(R(p2)), C(0)(p2,p3), ..., ∗(R(pm−1)), C(0)(pm−1, pm), ∗(R(pm))

// while not IsEmpty(Q) do
//     p ← DeleteMin(Q)
//     case p of
//     p is a site in ∗(V):
//         find the occurrence of a region ∗(R(q)) in T containing p,
//           bracketed by C(r,q) on the left and C(q,s) on the right
//         create new boundary rays C(-)(p,q) and C(+)(p,q) with bases p
//         replace ∗(R(q)) with ∗(R(q)), C(-)(p,q), ∗(R(p)), C(+)(p,q), ∗(R(q)) in T
//         delete from Q any intersection between C(r,q) and C(q,s)
//         insert into Q any intersection between C(r,q) and C(-)(p,q)
//         insert into Q any intersection between C(+)(p,q) and C(q,s)
//     p is a Voronoi vertex in *(V):
//         let p be the intersection of C(q,r) on the left and C(r,s) on the right
//         let C(u,q) be the left neighbor of C(q,r) and
//           let C(s,v) be the right neighbor of C(r,s) in T
//         create a new boundary ray C(0)(q,s) if q.y = s.y,
//           or create C(+)(q,s) if p is right of the higher of q and s,
//           otherwise create C(-)(q,s)
//         replace C(q,r), ∗(R,r), C(r,s) with newly created C(q,s) in T
//         delete from Q any intersection between C(u,q) and C(q,r)
//         delete from Q any intersection between C(r,s) and C(s,v)
//         insert into Q any intersection between C(u,q) and C(q,s)
//         insert into Q any intersection between C(q,s) and C(s,v)
//         record p as the summit of C(q,r) and C(r,s) and the base of C(q,s)
//         output the boundary segments C(q,r) and C(r,s)
//     endcase
// endwhile

// output the remaining boundary rays in T

// https://en.wikipedia.org/wiki/Fortune's_algorithm

