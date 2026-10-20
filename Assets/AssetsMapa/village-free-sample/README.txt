VILLAGE — free sample
The picket fence as a full autotile (16 pieces and 2 gates) and 8 props, in their new look.
The full pack has 37 kinds of prop, 4 variants of each, 6 fence styles, and 5 looks of
everything (weathered, mossy, snowy, night): https://mmorwynn.itch.io/village-pack

props/
  small/<kind>/   14 m = 96 px, the scale of the HOUSES pack's small set
  large/<kind>/   14 m = 192 px, the scale of its large set
                  <kind>_<variant>_<look>.png and ..._shadow.png, the same size, lining up exactly

fences/
  small/<style>/  a cell's diamond is 32 x 16 px
  large/<style>/  a cell's diamond is 64 x 32 px
                  <style>_<piece>_<look>.png and ..._shadow.png

FENCES THAT JOIN
Each fence is an autotile: one piece per grid cell, a post in the middle and an arm toward every
neighbour it joins. The piece is named by its arms, as they point on screen:

  post             no neighbours            ne, se, sw, nw         an end
  ne_sw, se_nw     a straight run           ne_se, se_sw, ...      a corner
  ne_se_sw, ...    a T                      ne_se_sw_nw            a crossing
  gate_ne_sw, gate_se_nw                    a gate across a straight run

Pick a cell's piece from which neighbours have fence, as any autotiler does (Godot terrain sets,
Tiled's Wang sets, RPG Maker style bitmasks: NE=1, SE=2, SW=4, NW=8). Every piece of a style is
drawn in the same frame, so on a 2:1 grid they meet pixel for pixel. Draw cells back to front.

PIVOTS
manifest.json gives every image's pivot (the pixel under the middle of the prop, or of the cell)
for both sizes. Anchor sprites there.

LICENCE
Use in any project, commercial or not. Credit welcome, not required. Do not resell or
redistribute the files themselves as an asset pack.
