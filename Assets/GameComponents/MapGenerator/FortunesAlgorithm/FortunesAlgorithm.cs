using System;
using System.Collections.Generic;
using System.Linq;
using FortunesAlgoritmGeometry;
using UnityEngine;

//#nullable enable // Unity doesnt like this

public class FortunesAlgorithm {
    private List<Boundary> V;
    private List<Region> A;

    public FortunesAlgorithm(List<Coordinates> siteCoordinates) {
        V = new List<Boundary>();
        A = new List<Region>();

        List<Site> S = siteCoordinates.ConvertAll<Site>(p => new Site(p.x, p.y));
        S.Sort();

        FortunesPlaneSweepingAlgorithm(S);
    }

    private void FortunesPlaneSweepingAlgorithm(List<Site> S) {
        List<Site> initialSites = InitialSites(S);
        List<Point> Q = InitialQ(initialSites, S);
        List<PointSet> T = InitialT(initialSites);

        while(Q.Any()) {
            Point p = DeleteMax(Q);

            if(p.GetType()==typeof(Site)) {
                SiteEvent((Site)p, Q, T);
            }
            else if(p.GetType()==typeof(Vertex)) {
                CircleEvent((Vertex)p, Q, T);
            }
        }

        foreach(Boundary C in T.OfType<Boundary>()) {
            AddIfNotInList(V, C.Normal());
        }
    }

    // ================================== Site Event

    private void SiteEvent(Site p, List<Point> Q, List<PointSet> T) {
        Region Rq;
        int index;
        Boundary Crq; //Boundary?
        Boundary Cqs; //Boundary?

        (Rq, index, Crq, Cqs) = FindRegion(p, T); //Find region under p
        Site q = Rq.site;

        Boundary Cpq = new Boundary(p, q);

        List<PointSet> additionT = new List<PointSet>(){Cpq.Neg(), new Region(p), Cpq.Pos(), Rq};
        T.InsertRange(index+1, additionT);

        if(Crq != null && Cqs != null) { Q.RemoveAll(point => Boundary.IsIntersection(Crq, Cqs, point)); }
        if(Crq != null) { AddIfNotNull(Q, Crq.Intersection(Cpq.Neg())); }
        if(Cqs != null) { AddIfNotNull(Q, Cpq.Pos().Intersection(Cqs)); }
    }

    private (Region, int, Boundary, Boundary) FindRegion(Site p, List<PointSet> T) { //Boundary? Crq, Boundary? Cqs
        Region Rq = Region.InRegion(T.OfType<Region>().ToList(), p);
        Boundary Crq = null; //Boundary?
        Boundary Cqs = null; //Boundary?

        int index = T.IndexOf(Rq);
        if(index > 0) { Crq = T[index-1] as Boundary; }
        if(index < T.Count - 1) { Cqs = T[index+1] as Boundary; }

        return (Rq, index, Crq, Cqs);
    }

    // ================================== Circle Event

    private void CircleEvent(Vertex p, List<Point> Q, List<PointSet> T) {
        Boundary Cqr;
        Boundary Crs;
        int index;
        Boundary Cuq; //Boundary?
        Boundary Csv; //Boundary?

        (Cqr, Crs, index, Cuq, Csv) = FindIntersection(p, T);
        Site q = Cqr.LeftSite;
        Site s = Crs.RightSite;

        Boundary Cqs = new Boundary(q, s);
        if(q.y > s.y) { Cqs = Cqs.Neg(); }
        else if(q.y < s.y) { Cqs = Cqs.Pos(); }
        else { Cqs = Cqs.Zero(); }

        T.RemoveRange(index, 3);
        T.Insert(index, Cqs);

        CloseVertex(p, Cqr, Crs, Cqs);

        if(Cuq != null) { Q.RemoveAll(point => Boundary.IsIntersection(Cuq, Cqr, point)); }
        if(Csv != null) { Q.RemoveAll(point => Boundary.IsIntersection(Crs, Csv, point)); }
        if(Cuq != null) { AddIfNotNull(Q, Cuq.Intersection(Cqs)); }
        if(Csv != null) { AddIfNotNull(Q, Cqs.Intersection(Csv)); }
    }

    private (Boundary, Boundary, int, Boundary, Boundary) FindIntersection(Vertex p, List<PointSet> T) { //Boundary? Cuq, Boundary? Csv
        Boundary Cqr = p.LeftBoundary;
        Boundary Crs = p.RightBoundary; // TODO check this
        Boundary Cuq = null; //Boundary?
        Boundary Csv = null; //Boundary?

        int index = IndexOfBoundary(T, Cqr);
        if(!Crs.Equals(T[index+2])){
            throw new Exception("Incorrect Boundaries in FindIntersection");
        }
        if(index > 1) { Cuq = T[index-2] as Boundary; }
        if(index < T.Count - 4) { Csv = T[index+4] as Boundary; }

        return (Cqr, Crs, index, Cuq, Csv);
    }

    private void CloseVertex(Vertex p, Boundary Cqr, Boundary Crs, Boundary Cqs) {
        // TODO remember this moment as the moment a tile got closed and borders made
        Cqr.Summit = p;
        Crs.Summit = p;
        Cqs.Base = p;

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

    private static int IndexOfBoundary(List<PointSet> T, Boundary C) {
        for(int index = 0; index < T.Count; index++) {
            Boundary elem = T[index] as Boundary;
            if(elem != null && C.Equals(elem)) { return index; }
        }
        throw new Exception("Border not found!");
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

    // ==================================== Helper Functions (use these before you want result)

    public FortunesAlgorithm RemoveShortBoundaries(float minLenght) { // TODO not urgent
        foreach(Boundary C in V){
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

    public FortunesAlgorithm CutCorners(Rect bounds) {
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

        // Step 1: Setting the borders & tiles
        foreach(Boundary C in V) {
            borders.Add(C.CreateUnsetBorder());
        }
        foreach(Region R in A) {
            tiles.Add(R.CreateUnsetTile());
        }

        // Step 2: Correcting the refrences // TODO not urgent
        foreach(UnsetBorderInit b in borders) {
            //GetBorderFromHash()
            //b.SetBorderInit();
        }
        foreach(UnsetTileInit t in tiles) {
            //t.SetTileInit();
        }

        return (borders, tiles);
    }

    private List<BorderInit> GetBorderFromHash(int code) {
        throw new Exception();
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


// add a site event in the event queue for each site
// while the event queue is not empty
//     pop the top event
//     if the event is a site event
//         insert a new arc in the beachline
//         check for new circle events
//     else
//         create a vertex in the diagram
//         remove the shrunk arc from the beachline
//         delete invalidated events
//         check for new circle events

// https://pvigier.github.io/2018/11/18/fortune-algorithm-details.html

