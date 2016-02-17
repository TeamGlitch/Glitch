u s i n g   U n i t y E n g i n e ;  
 u s i n g   S y s t e m . C o l l e c t i o n s ;  
 u s i n g   S y s t e m . C o l l e c t i o n s . G e n e r i c ;  
  
 p u b l i c   c l a s s   P l a y e r C o n t r o l l e r   :   M o n o B e h a v i o u r  
 {  
 	 p u b l i c   f l o a t   s p e e d   =   3 0 . 0 f ;  
 	 p u b l i c   f l o a t   j u m p S p e e d   =   1 0 0 . 0 f ;  
 	 p u b l i c   f l o a t   g r a v i t y   =   9 . 8 f ;  
  
 	 p r i v a t e   f l o a t   v S p e e d   =   0 . 0 f ;  
 	 p r i v a t e   V e c t o r 3   m o v e D i r e c t i o n   =   V e c t o r 3 . z e r o ;  
 	 p r i v a t e   i n t   n u m B o x e s   =   0 ;  
  
 	 p u b l i c   G a m e O b j e c t   e r r o r B o x P r e f a b ;  
 	 C h a r a c t e r C o n t r o l l e r   c o n t r o l l e r ;  
  
 	 / /   U s e   t h i s   f o r   i n i t i a l i z a t i o n  
 	 v o i d   S t a r t   ( )  
 	 {  
 	 	 c o n t r o l l e r   =   G e t C o m p o n e n t < C h a r a c t e r C o n t r o l l e r >   ( ) ;  
 	 }  
  
 	 / /   U p d a t e   i s   c a l l e d   o n c e   p e r   f r a m e  
 	 v o i d   U p d a t e   ( )  
 	 {  
 	 	 m o v e D i r e c t i o n   =   n e w   V e c t o r 3   ( I n p u t . G e t A x i s   ( " H o r i z o n t a l " ) ,   0 ,   0 ) ;  
 	 	 m o v e D i r e c t i o n   =   t r a n s f o r m . T r a n s f o r m D i r e c t i o n   ( m o v e D i r e c t i o n ) ;  
 	 	 m o v e D i r e c t i o n   * =   s p e e d ;  
  
 	 	 i f   ( c o n t r o l l e r . i s G r o u n d e d )   {  
 	 	 	 i f   ( I n p u t . G e t B u t t o n   ( " J u m p " ) )  
 	 	 	 	 v S p e e d   =   j u m p S p e e d ;  
 	 	 	 e l s e  
 	 	 	 	 v S p e e d   =   0 ;  
 	 	 }  
  
 	 	 v S p e e d   - =   g r a v i t y   *   T i m e . d e l t a T i m e ;  
 	 	 m o v e D i r e c t i o n . y   =   v S p e e d ;  
 	 	 c o n t r o l l e r . M o v e   ( m o v e D i r e c t i o n   *   T i m e . d e l t a T i m e ) ;  
  
 	 	 i f   ( I n p u t . G e t M o u s e B u t t o n D o w n   ( 0 ) )   {  
 	 	 	 i f   ( n u m B o x e s   <   3 )   {  
 	 	 	 	 V e c t o r 3   m o u s e   =   I n p u t . m o u s e P o s i t i o n ;  
 	 	 	 	 m o u s e . z   =   1 5 ;  
 	 	 	 	 m o u s e   =   C a m e r a . m a i n . S c r e e n T o W o r l d P o i n t   ( m o u s e ) ;  
  
 	 	 	 	 G a m e O b j e c t   e r r o r B o x   =   ( G a m e O b j e c t ) I n s t a n t i a t e   ( e r r o r B o x P r e f a b ) ;  
 	 	 	 	 e r r o r B o x . t r a n s f o r m . p o s i t i o n   =   n e w   V e c t o r 3   ( m o u s e . x ,   m o u s e . y ,   0 ) ;  
 	 	 	 	 e r r o r B o x . G e t C o m p o n e n t < E r r o r B o x S c r i p t >   ( ) . d u r a t i o n   =   5 0 0 ;  
 	 	 	 	 e r r o r B o x . G e t C o m p o n e n t < E r r o r B o x S c r i p t >   ( ) . p l a y e r   =   t h i s ;  
 	 	 	 	 n u m B o x e s + + ;  
 	 	 	 }  
 	 	 }  
 	 }  
  
 	 p u b l i c   v o i d   e r r o r B o x D e l e t e d   ( i n t   n u m )  
 	 {  
 	 	 n u m B o x e s   - =   n u m ;  
 	 }  
  
 	 / *  
 	 v o i d   O n C o n t r o l l e r C o l l i d e r H i t ( C o n t r o l l e r C o l l i d e r H i t   c o l l ) { 	 	  
 	 	 i f   ( c o l l . g a m e O b j e c t . C o m p a r e T a g ( " F l o o r " ) )  
 	 	 {  
 	 	 	 / / T e x t u r e E f f e c t s . T e x t u r e F l i c k e r ( c o l l . g a m e O b j e c t ,   b r o k e n T e x t u r e ) ;  
 	 	 }  
 	 	 e l s e  
 	 	 {  
 	 	 	 / / T e x t u r e E f f e c t s . T e x t u r e F l i c k e r R e p e a t ( c o l l . g a m e O b j e c t ,   b r o k e n T e x t u r e ) ;  
 	 	 }  
 	 }  
  
 	 v o i d   O n C o l l s i o n E x i t ( C o l l i s i o n   c o l l )  
       	 {  
 	 	 T e x t u r e E f f e c t s . T e x t u r e R e m o v e ( c o l l . g a m e O b j e c t ,   b r o k e n T e x t u r e ) ;  
         }  
         * /  
 } 