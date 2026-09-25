# 2026q3-game-developer-exercise-vondergames-JiratthaKiatmonkong


Game Build Link : [https://drive.google.com/file/d/1fkbkjU0MSMFST1agKr1LQYwXxoEBWnbi/view?usp=sharing](https://drive.google.com/file/d/1fkbkjU0MSMFST1agKr1LQYwXxoEBWnbi/view?usp=sharing)

Video Link : [https://drive.google.com/file/d/1liThZtxTRGB3K29YGe5tptreLC\_QFDQw/view?usp=sharing](https://drive.google.com/file/d/1liThZtxTRGB3K29YGe5tptreLC_QFDQw/view?usp=sharing)


###### September 23th 2026 (Day 1)



=> 15.00

&#x09;- Install Unity Version (6000.3.15f1)

=> 16.00

&#x09;- Create GitHub Repository \& Unity Project (2D)

&#x09;- Create a simple environment for develop required systems (16.15 - 16.30)



=> System 1 Task \[Time Hop System]

&#x09;- Modifying Visual Art Asset for day periods. (17.11 - 17.45)

&#x09;- Make Script.cs for trigger Day Change Period by Keyboard (17.45 - 18.15)

&#x09;- Make Basic Movement for PlayerController (18.50 - 19.35)

&#x09;- Make EnvironmentTrigger by press F when player in range functional (19.35 - 20.00)

&#x09;- Make Period Change Automatically + Make System 1 Fully Functional (21.00 - 22.00)



###### September 23th 2026 (Day 2)



=> System 2 Task \[Inventory System]

&#x09;- Create a simple Item object to collect in the scene. (07.30 - 07.35)

&#x09;- Make Scripts.cs about Item and Inventory. (11.35 - 12.00)

&#x09;- Make Simple UI MockUp for Inventory (12.00 - 13.15)

&#x09;- Make the Inventory UI and other related script.cs functional (13.30 - 17.40)

&#x09;- Make the hotbar usage functional by clicked or HotKey (21.00 - 21.30)



=> System 4 Task \[Crafting System]

&#x09;- Make Script.cs related to CraftingRecipeData and CraftingManager (13.45 - 15.00)

&#x09;- Make the Script.cs related to Crafting System functional (19.00 - 20.30)

&#x09;- Make the crafting station functional and not interfere with the Instant Crafting system (20.45 - 21.00)



=> System 3 Task \[Combat System]

&#x09;- Make the whole Combat System including EnemySlime, MagicWand, PlayerHealth, CombatArea etc. functional (21.00 - 22.45)





จากที่ทำระบบมาทั้งหมด 4 ระบบ ขออนุญาตรีวิวให้ดูนะครับ ว่าแต่ละระบบเป็นอย่างไร

แต่การทำงานโค้ดเหล่านี้จะมีการใช้ AI (Gemini) ขึ้นโครงมาให้แล้วมาปรับให้เป็นไปความต้องการของเรา

แล้วก็วนไปให้ AI ขัดเกลาหรือให้เขาแก้ Bug Error ให้เพื่อประหยัดเวลาไม่ต้องไปดู Documentation ของ Unity น่ะครับ 



System 1 Task \[Time Hop System] (ความยาก 4/10)

ระบบนี้ในตอนแรกที่วางแผนผมได้ลองนำ Art Asset มาเปลี่ยนสีโดยใส่ Script.cs เกี่ยวกับการเปลี่ยนสีตาม Enum ของเวลาที่ผ่านไป

แต่พอคิดดูแล้ว ต้องมีปัญหาเรื่อง Performance แน่ ๆ เลยเปลี่ยนแผนมาเล่นเรื่องแสง URP แล้วเล่นเรื่องสีของแสงนั้นน่าจะดีกว่า

ซึ่งพอได้ลองทำด้วยเทคนิค URP ก็ได้ผลจริง ๆ 
และในส่วนของการ Debug เพื่อดูข้อมูลของ จำนวนวัน (Day1-100++), ลำดับวัน (Mon-Sun), ช่วงเวลา (Morning-Evening), เวลา (00.00 - 23.59)

ผมจะแสดงเป็นในรูปแบบ Text บนหน้าจอแบบ Realtime ไว้ละกันครับ



System 2 Task \[Inventory System] (ความยาก 8/10)

เนื่องจากผมเคยทำระบบ Inventory ที่คล้ายกันในเกมที่ผมเคยทำ ก็เลยทำให้สามารถจินตนาการออกว่าต้องทำระบบคร่าว ๆ อย่างไร

แต่แน่นอนว่าที่ผมระบบ Inventory System ที่ผมทำมามันคงมีอะไรให้ปรับปรุงอีกเยอะเลยล่ะครับ 

ทั้ง User Expeirence ยังต้องการปรับให้การ Navigate UI เป็น Universal มากขึ้น หรือให้ใช้งานง่ายขึ้น

แต่ ณ ตอนนี้ระบบ inventory รวมถึงการ Navigate UI อื่น ๆ คงเป็นการใช้แบบ Gamepad ไม่ได้ ตอนนี้ใช้ได้เป็นแบบ Mouse Click เท่านั้น



ยังมีปัญหาเรื่องการทำให้จำนวนช่อง Inventory มีความยืดหยุ่นต่อการใช้ Vertical Slider อยู่ ยังไม่มีเวลามากพอที่จะทำให้การทำงานตรงนี้ Scalable ได้

แต่ที่มี ณ ตอนนี้จะเป็นข้อความ Log ทางขวามือของหน้าจอจะช่วย Info กับท่านได้ว่า เออ ผู้เล่นกดอันนี้ไปแล้วผู้เล่นกำลังทำอะไรอยู่ ประมาณนี้ครับ
(เดี๋ยวผมทำ Control Hint ไว้ให้ครับ จะได้ไม่งงตอนทดสอบ)



System 3 Task \[Combat System] (ความยาก 3/10)

เนื่องจากที่ว่ามีประสบการณ์การออกแบบ Combat ที่คล้าย ๆ กันในเกม 2D Top Down ที่เคยทำมาก่อน

เลยสามารถที่จะ Reuse โค้ดเก่าที่มี เอามาปรับใช้ได้โดยที่ไม่ต้องทำจาก Scratch ใหม่หมด

ในใจอยากใช้เป็น Overlap มากกว่าเพื่อ Performance แต่เนื่องจากเวลาที่ผมสะดวกในการทำแบบทดสอบนี้ใกล้หมดแล้ว

เลยใช้ Collider แก้ปัญหาไปก่อนน่ะครับ



System 4 Task \[Crafting System] (ความยาก 6/10)

อันนี้จะเชื่อมโยงมาจาก Inventory เลยครับ ก็ทำไว้สองแบบครับคือแบบ instant กับแบบ stationary 

ก็มีปัญหาเรื่อง UX ที่ว่า UI ของ Inventory และ Crafting มันทับกันก็เลยแก้ขัดด้วยการปิดเมนูของกันและกัน

เวลาเปิดเมนูใดเมนูหนึ่งขึ้นมาน่ะครับ

แต่ปัญหาเรื่อง Scalable ที่มี ณ ตอนนี้คือ Item ที่ Craft ได้นั้นสามารถมีวัตถุดิบได้เพียงสูงสุดแค่ 5 ชนิดเท่านั้น

&#x20;







&#x09;



&#x09;

