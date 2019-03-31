
//  <formula 5; \infty>

// <formula \bigcup_{lower}^{upper}>


// <formula \bigcap_{lower}^{upper}>

// <formula \coprod_{lower}^{upper}>


// <formula {}>

// <formula \frac{1}{2}>

// <formula \lim_{x\to\infty}>

// <formula \left(\right)>

// <formula \sqrt[3]{2}>

// <formula \circ>

// <formula \oint>

// <formula \prod_{lower}^{upper}>

// <formula \sum_{lower}^{upper}>

// <formula \sqrt{-1}>

// <formula \vec{AB}>


// The quadratic equation: <formula 2; x=\frac{-b \pm \sqrt{b^2-4ac}}{2a}>  This is super cool.


// Math Symbols:
// <formula \lt>
// <formula \leq>
// <formula \gt>
// <formula \geq>
// <formula \ll>
// <formula \gg>
// <formula \subset>
// <formula \subseteq>



// <formula a < b>




// <formula a'''>

// <formula \frac{\mathrm d}{\mathrm d x} \left( k g(x) \right)>


// <formula \pm>


// <formula \mp>



// <formula \left(\frac{x^2}{y^3}\right)>


// <formula \left.\frac{x^3}{3}\right|_0^1>




// <formula \frac{n!}{r!(n-r)!}>

// <formula \frac{1}{2}, \frac{2}{3}, \frac{5}{6}>

// <formula x = a_0 + \frac{1}{a_1 + \frac{1}{a_2 + \frac{1}{a_3 + \frac{1}{a_4} } } }>

// <formula \top \bot \emptyset >

// <formula \frac{\partial }{\partial x}>

// <formula \prod_{a}^{b}>


// <formula \prod_{n=1}^{10}n^2>



// Dots:
// ldots: <formula \ldots>
// cdots: <formula \cdots>



// Sets:

// <formula \bigcup_{i=1}^{n}{X_i}>

// <formula \bigcap_{i=1}^{n}{X_i}>


// Accents:
// <formula \overline{aaa}>
// <formula \bar{a}>
// <formula \check{a}>
// <formula \grave{a}>
// <formula \hat{a}>
// <formula \acute{a}>
// <formula \ddot{a}>
// <formula \dot{a}>
// <formula \not{a}>
// <formula \widetilde{AAA}>
// <formula \widehat{AAA}>
// <formula \tilde{a}>
// <formula \underline{a}>
// <formula \vec{a}>
// <formula \breve{a}>



// Trig:

// <formula \cos^{-1}\theta>

// <formula \sin^{-1}\theta>

// <formula \left(\frac{\pi}{2}-\theta \right )>

// <formula e^{i \theta}>



// Geometry: 
// <formula \vec{AB}>  <- Work-around for unrecognized \overrightarrow.

// <formula \widehat{AB}>

// <formula \Delta A B C >


// Physics:
// <formula \vec{F}=m\vec{a}>
// <formula e=m c^2>
// <formula \vec{F}=m \frac{d \vec{v}}{dt} + \vec{v}\frac{dm}{dt}>
// <formula \oint \vec{F} \cdot d\vec{s}=0>



// <formula x = a_0 + \frac{1}1 \frac{1}{2}{a_1 + \frac{1}{a_2 + \frac{1}{a_3 + a_4}}}>

// <formula \left(x-1\right)\left(x+3\right) \sqrt{a^2+b^2}>

// <formula x = a_0 + \frac{1}{a_1 + \frac{1}{a_2 + \frac{1}{a_3 + a_4}}}>

// <formula \sqrt{\Gamma} \cdot \sqrt[3]{\gamma}>

// <formula \sqrt{-1} \cdot \sqrt[3]{\Gamma}>





// <formula \alpha \beta \gamma \Gamma \delta \Delta \epsilon \zeta > 
// <formula \eta \theta \Theta \iota \kappa \lambda \Lambda \mu \nu \omicron \pi \Pi        >  

// <formula \rho \sigma \Sigma \tau \upsilon \Upsilon \phi \Phi \chi \psi \Psi \omega \Omega             > 


// <formula \frac{\partial y}{\partial x}>


// <formula \frac{d}{dx}c^n=nx^{n-1}>


// <formula \frac{d}{dx}e^{ax}=ae^{ax}>

// <formula \frac{d}{dx}\ln(x)=\frac{1}{x}>

// <formula \frac{d}{dx}\sin x=\cos x>

// <formula \frac{d}{dx}\cos x=-\sin x>

// <formula \frac{d}{dx}\tan x=\sec^2 x>

// <formula \frac{d}{dx}\cot x=-\csc^2 x>

// <formula \int u \frac{dv}{dx} dx=uv-\int \frac{du}{dx}v*dx>

// <formula \frac{n!}{r!(n-r)!}>

// <formula \sum_{i=1}^{n}{X_i}>

// <formula \sum_{i=1}^{n}{X_i^2}>

// <formula \frac{1}{2} \sum_{i=2}^{n}>

// <formula \prod_{i=a}^{b}>

// <formula \lim_{i\to\infty}>

// <formula \sum_{n=1}^{\infty}>

// <formula \int_V \mu(u,v) du dv>

// <formula \sum_{i=1}^{n}{(X_i - \overline{X})^2}>

// <formula X_1, \cdots,X_n >

// <formula \frac{x-\mu}{\sigma}>


// <formula \bigcup_{i=1}^{n}>

// <formula \bigcap_{i=1}^{n}X_i>

// <formula \bigcap_{i=1}^{n}{X_i}^2>

// <formula \cos^{-1}\theta>

// <formula \sin^{-1}\theta>

// <formula e^{i \theta}>

// <formula \left(\frac{\pi}{2}-\theta \right)>


// <formula 100^{\circ}C>

// <formula \vec{F}=m \vec{a}>

// <formula e=m c^2>

// <formula \vec{F}=m \frac{d \vec{v}}{dt} + \vec{v}\frac{dm}{dt}>

// <formula \oint \vec{F} \cdot d\vec{s}=0>

// <formula \vec{F_g}=-F\frac{m_1 m_2}{r^2} \vec{e_r}>

// <formula \frac{\mathrm{d} }{\mathrm{d} x}>

// <formula \sqrt[2]{-1}>

// <formula \prod_{a}^{b} >

// <formula \coprod>

// <formula a = \coprod_{a}^{b}>



public Tuple<double, double> SolveQuad(double a, double b, double c)
{
	// Solved with the Quadratic equation:

	//`  <formula 8; x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}> 

	// <formula k_{n+1} = n^2 + k_n^2 - k_ {n-1}>

	// <formula f(n) = n^5 + 4n^2 + 2 |_{n=17}>

	// <formula \frac{\frac{1}{x}+\frac{1}{y}}{y-z}>


	//! Issues with square roots and nth roots:

	// <formula 8; \sqrt[3/7]{-1}>

	// <formula 8; \sqrt[1234]{-1}>



	double sqrt4ac = Math.Sqrt(4 * a * c);

	return new Tuple<double, double>((-b + sqrt4ac) / (2 * a),
		(-b - sqrt4ac) / (2 * a));
}


// <formula \sqrt[n]{1+x+x^2+x^3+\cdots+x^n}>

// <formula \sum_{i=1}^{10} t_i>


// Big Square Cup: <formula \bigsqcup>


// <formula \bigodot>
// Big O Plus: <formula \bigoplus>

// Big O Times: <formula \bigotimes>

// Big Vee: <formula \bigvee>

// Big Wedge: <formula \bigwedge>

// Nabla: <formula \nabla>

// Down arrow: <formula  \downarrow>
// Exists: <formula  \exists>
// For All: <formula  \forall >
// Neg: <formula  \neg >
// Subset: <formula  \subset 	>
// Superset: <formula  \supset>
// In: <formula  \in>
// Ni: <formula  \ni>
// Land: <formula  \land >
// Lor: <formula  \lor >
// Right arrow/to: <formula \to and \rightarrow>
// Left arrow/gets: <formula \leftarrow  and \gets >
// Double right arrow: <formula  \Rightarrow>
// Left/right arrow: <formula  \leftrightarrow>
// Double left/right arrow: <formula  \Leftrightarrow>


// <formula \top \bot \emptyset \xi>

// <formula \lceil \rceil \lfloor \rfloor>

// <formula \langle \rangle>


// <formula \|>


// <formula \uparrow \downarrow \Uparrow \Downarrow>


//`!------------------------------------
// Rendering issues:
//! The formulas below appears to fail under our 1.0 rendering engine but work correctly at https://www.codecogs.com/latex/eqneditor.php.
//! PNG Images taken as screenshots from the online equation editor are pasted into the code, below our formulas for reference.



// <formula \int_0^\infty \mathrm{e}^{-x}\,\mathrm{d}x>

// ![](39F89D5CD3F8F110BBB95F20381E5959.png)




// <formula \left \{ A \right \}>

// ![](61485E425C07C630121F76F0E90E5797.png)




// <formula {^n}C_r>

// ![](6D3554828437C38DBA691852AEAAF7CE.png)




// <formula \frac{d}{dx}e^{ax}=a\,e^{ax}>

// ![](F8F764052C665D163C7197254F96E64D.png)




// <formula \begin{pmatrix} a_{11} & a_{12} \\ a_{21} & a_{22} \end{pmatrix}>

// ![](80D7674EA0259CF6DE72BBF353F07394.png)




// <formula \begin{pmatrix} a_{11} & a_{12} & a_{13}\\ a_{21} & a_{22} & a_{23}\\ a_{31} & a_{32} & a_{33} \end{pmatrix}>

// ![](DF1B14EEE718528B4E888790654698A7.png)




// <formula \begin{pmatrix} a_{11} & \cdots & a_{1n}\\  \vdots & \ddots & \vdots\\ a_{m1} & \cdots & a_{mn} \end{pmatrix}>

// ![](0C89F2C33136B7CCEA7231064FEF288B.png)




// <formula \begin{pmatrix} 1 & 0 \\ 0 & 1 \end{pmatrix}>

// ![](AD67D5B3EBCD1B89E7F69606C92D7CA3.png)




// <formula \overrightarrow{AB}>

// ![](1C266671F985807054A433F3F4F2D448.png)




// <formula \overleftrightarrow{AB}>

// ![](FADC6F38484C336D98AAAC7660C3DE07.png)





// <formula _{10}^{5}C^{16}>

// ![](81388D08116CD3512EADF3721BBC3AB2.png)




// <formula 2H_2 + O_2 \xrightarrow{n,m}2H_2O>

// ![](B28D8B5F43614A0CD23322C0F615433E.png)




// <formula A\underset{b}{\overset{a}{\longleftrightarrow}}B>

// ![](D5B4146B90CFDBB403BA9C143457CCF3.png)



// <formula A\underset{0}{\overset{a}{\rightleftarrows}}B>

// ![](B070CEB13BE13DD03D06964BCFA0D599.png)




// <formula A\underset{0^{\circ}C }{\overset{100^{\circ}C}{\rightleftarrows}}B>

// ![](7644DED2B5217541FB18105BDF582304.png)




// <formula \vec{F}_g=-F\frac{m_1 m_2}{r^2} \vec{e}_r>

// ![](319965B399BF084BA4493886989922F1.png)




// <formula \{ \} >

// ![](DE121BB6A5232720A867DA8894C34C2F.png)




// <formula \varnothing>

// ![](56A136B2C1E235FE12480A406409E012.png)




// <formula \iff>

// ![](553103C915F97298C8CA72246ABB85FB.png)




// <formula \implies >

// ![](19F278EBEB7B24CE7F959A5E1D692145.png)




// <formula \mapsto >

// ![](9B91939C1D3F7435622CEAE946796242.png)




// <formula  \nexists>

// ![](AE3A894B9803985E3290C0BABB96C751.png)




// <formula \notin >

// ![](BB1D3A26B08BD14A6BA115B3AC20AD32.png)




// <formula \idotsint>

// ![](675E5A926D32B7E413B2FAF1BC15491E.png)




// <formula \boldsymbol{A}+B>

// ![](FF8F6AD1405B92E75CB69199A59F8C0A.png)




// <formula \textup{A}A>
// ![](85306BD265F6ECDD7B9A09C8ED756222.png)




// <formula \textrm{z}\textsl{y}\texttt{t}>

// ![](87A32D026B13E9E5609B877186EC2F51.png)




// <formula 1 \tfrac{1}{2}>

// ![](23E996035F21EB364D5974CEF77DCF23.png)




// <formula 1 \mathbf{\tfrac{1}{2}}>

// ![](BD74D9081EFA217D64DB67A565C6550C.png)




// <formula \sqrt[n]{1+x+x^2+x^3+\dots+x^n}>

// ![](0BCA9027D9A2E6ABB3941BFE6EACEE0A.png)




// <formula \sum_{\substack{0<i<m \\ 0<j<n }}  P(i, j)>

// ![](C3FDC2AA39D4F1591097B1F2240366E8.png)




// <formula \int\limits_a^b>

// ![](54D80EB8A13FC9FD7286ABBB9FE6F4DF.png)




// <formula \iiint>

// ![](ADABB05703658A5A790F30A17E571996.png)




// <formula \iint>

// ![](32954A6B5E0E06100BB3FDDD615FEFF9.png)




// <formula \idotsint>

// ![](675E5A926D32B7E413B2FAF1BC15491E.png)




// <formula ( a ), [ b ], \{ c \}, | d |, \| e \|, \langle f \rangle, \lfloor g \rfloor, \lceil h \rceil, \ulcorner i \urcorner>

// ![](44F7A0F59F7A29F0AB61B04901703DA3.png)





// <formula P\left(A=2\middle|\frac{A^2}{B}>4\right)>

// ![](A57DE55473D2B62E1E13C65ACAE2D5FC.png)




// <formula \left\{\frac{x^2}{y^3}\right\}>

// ![](C15C855D1600D4F9821DACB1D5791916.png)




// <formula ( \big( \Big( \bigg( \Bigg(>

// ![](F8F6B876DF4BB390EB378C53C0AE2B4E.png)




// <formula \frac{\mathrm d}{\mathrm d x} \big( k g(x) \big)>

// ![](38940F5A736769CD9FBE58D606ACBACE.png)




// <formula \begin{array}{c|c} 1 & 2 \\ \hline 3 & 4 \end{array}>

// ![](3F94BFF02B8B99CDC994B1D523F95400.png)




// <formula M = \bordermatrix{~ & x & y \cr A & 1 & 0 \cr B & 0 & 1 \cr}>

// ![](7ECC752970AF1E83339641EACB916538.png)




// <formula 50 \textrm{ apples} \times 100 \textbf{ apples} = \textit{lots of apples}^2>

// ![](147AD3B0406468EBE1245E99A7B8B907.png)




// <formula \boldsymbol{\beta} = (\beta_1,\beta_2,\dotsc,\beta_n)>

// ![](85AE04AAFECD903A2A6AC0F1703A4DB4.png)




// <formula \overleftrightarrow{AB}>

// ![](FADC6F38484C336D98AAAC7660C3DE07.png)




// <formula {\rightleftarrows}>

// ![](BBB7CF03077D2AA8A2E64490D07EE822.png)




// <formula \overleftarrow{AB}>

// ![](E47ED3EAA2415998063F3AC1693F0438.png)




// <formula \mathring{a}>

// ![](CF0F55385D7923348E305699197A8A1B.png)




// <formula \stackrel\frown{AAA}>

// ![](4470586160DE497D3FAC87F884D887AD.png)




// <formula \dddot{a}>

// ![](9D951CB6B3B0C97D1C89E4DFF355C236.png)




// <formula \ddddot{a}>

// ![](9063B129849C06FFCDE898FF1E5740C5.png)




// <formula f(n) = \begin{cases} n/2       & \quad \text{if } n \text{ is even}\\ -(n+1)/2  & \quad \text{if } n \text{ is odd} \end{cases}>

// ![](5B62B24225EDB52F59E1C29280FE388C.png)





// <formula \left(\begin{ array}{c} n \\ r \end{array} \right) = \frac{n!}{r!(n-r)!}>

// ![](7AB6F44489348955D0C20CDC3E40530C.png)





// <formula \dots>

// ![](F002EDA420BAF57E30793601CFB182B0.png)




// <formula \vdots>

// ![](70166BEA7BBC033BB9BECA63B95F7DA7.png)




// <formula \ddots>

// ![](4A3BC1C41C3881D846360F2C3C15D839.png)




// <formula \begin{pmatrix} a_{11} & a_{12} & a_{13} & a_{14}\\ a_{21} & \hdotsfor{2} & a_{24}\\ a_{31} & a_{32} & a_{33} & a_{34}\end{pmatrix}>

// ![](87B3CD218C982ED66A5EB6A9EDEE55F9.png)




// <formula A_1,A_2,\dotsc,>
// ![](98837F90701196D306250EF9CBB5636D.png)





// <formula A_1+\dotsb+A_N>

// ![](6903363A5C9C558AE0F8D0A68F5AE44E.png)




// <formula A_1 \dotsm A_N>

// ![](BBD947584285B7879D7A27D128F70F4A.png)




// <formula \int_a^b \dotsi>

// ![](BA2933C2AA1FADC939BF3B4E71F17467.png)




// <formula A_1\dotso A_N>

// ![](2EDAA2A0A49E5AA02549149B1FB963E6.png)




// <formula \nsubseteq>
// ![](FE906AA905F5B483D07146431BC9A0F5.png)




// <formula \sqsubset>

// ![](4DD693B3D8165E78C5022EB0F94ECFC9.png)



