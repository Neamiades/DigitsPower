//#region 19
//function [ x3, y3, z3 ] = Point_Multiplication_Affine_Coord_19 (x1, y1, z1, a, k, p)


//    a_max = 15;
//    b_max = 17;
//    mas_k = zeros(1,3);
//    [ mas_k ] = Convert_to_DBNS_1 (k, a_max, b_max);
//%     [ mas_k ] = Convert_to_DBNS_2 (k, a_max, b_max);
    
//    % перевіримо чи правильно виконали розкладення
//    sum = 0;
//    s = size(mas_k);
//    for i = 1:s(1)
//        sum = sum + mas_k(i,1)*2^mas_k(i,2)*3^mas_k(i,3);
//    end;
    
//    mas_k
//    if (k ~= sum)
//        disp('ALERT');
//    end

   
//    t = size(mas_k);
    
//    x2 = x1;
//    y2 = y1;
//    z2 = z1;
    
//    for j = 1:mas_k(t(1),3)
//        [ x2, y2, z2 ] = Ternary_Affine_Coord_2 (x2, y2, z2, a, p);
//    end;
    
//    for j = 1:mas_k(t(1),2)
//        [ x2, y2, z2 ] = Double_Affine_Coord (x2, y2, z2, a, p);
//    end;
    
//    x3 = x2;
//    y3 = mas_k(t(1),1)*y2;
//    z3 = z2;
        
//    for i = t(1)-1:-1:1
//        u = mas_k(i, 2) - mas_k(i + 1, 2);
//        v = mas_k(i, 3) - mas_k(i + 1, 3);
        
//        for j = 1:u
//            [ x2, y2, z2 ] = Double_Affine_Coord (x2, y2, z2, a, p);
//        end;
        
//        for j = 1:v
//            [ x2, y2, z2 ] = Ternary_Affine_Coord_2 (x2, y2, z2, a, p);
//        end;
        
//        [ x3, y3, z3 ] = Add_Affine_Coord (x2, mas_k(i,1)*y2, z2, x3, y3, z3, a, p);
//    end;

    
//    if ~x3 && y3
//        z3 = 0;
//    else
//        z3 = 1;
//    end;
    
//end



//function [ mas_k ] = Convert_to_DBNS_1 (k, a_max, b_max)

//    i = 0;
//    s = 1;    
//    while k > 0
//        i = i + 1;
//        [ a, b ] = Best_Approximation_1 (k, a_max, b_max);
                         
//        a_max = a;
//        b_max = b;
//        z = 2^a * 3^b;
//        mas_k(i, 1) = s;
//        mas_k(i, 2) = a;
//        mas_k(i, 3) = b;
        
//        if k < z
//            s = -s;
//        end;
        
//        k = abs(k - z);      
//    end;

//end

//function [ mas_k ] = Convert_to_DBNS_2 (k, a_max, b_max)

//    i = 0;
//    s = 1;    
//    while k > 0
//        i = i + 1;
//        [ a, b ] = Best_Approximation_2 (k, a_max, b_max);
                         
//        a_max = a;
//        b_max = b;
//        z = 2^a * 3^b;
//        mas_k(i, 1) = s;
//        mas_k(i, 2) = a;
//        mas_k(i, 3) = b;
        
//        if k < z
//            s = -s;
//        end;
        
//        k = abs(k - z);      
//    end;

//end



//function [ a, b ] = Best_Approximation_1 (k, a_max, b_max)

//    min_x = a_max;
//    y = round(-min_x * log_dif_base(3, 2) + log_dif_base(3, k));
    
//    if y > b_max
//        y = b_max;
//    elseif (y < 0)
//        y = 0;
//    end;
//    min_delta = abs(y + min_x * log_dif_base(3, 2) - log_dif_base(3, k));
    
//    for x = 0:a_max
//        y = round(-x * log_dif_base(3, 2) + log_dif_base(3, k));
//        if y > b_max
//            y = b_max;
//        elseif (y < 0)
//            y = 0;
//        end;
        
//        delta = abs(y + x * log_dif_base(3, 2) - log_dif_base(3, k));
//        if min_delta > delta
//            min_x = x;
//            min_delta = delta;
//        end;
//    end;
    
//    a = min_x;
//    b = round(-min_x * log_dif_base(3, 2) + log_dif_base(3, k));
//    if b > b_max
//        b = b_max;
//    end;

//end

//function [ a, b ] = Best_Approximation_2 (k, a_max, b_max)

//    % цей алгоритм написаний за ідеями запропонованими у презентації
//    % Christophe Doche (Double-Base Number Systems and Applications), але
//    % деякі моменти було запропоновано мною
//    legth_k = get_number_bit(k, 2);
//    PreComputation = zeros(b_max + 1, 3);
//    for i = 0:b_max
//        PreComputation(i+1, 1) = i;
//        PreComputation(i+1, 2) = 3^i;
//        temp = get_number_bit(PreComputation(i+1, 2), 2);
//        PreComputation(i+1, 3) = bitshift(PreComputation(i+1, 2), legth_k - temp);
//    end;
    
//    % сортування PreComputation за останнім стовпцем (як в презентації)
//    for i = 1:b_max+1
//        imin = i;
//        min = PreComputation(i, 3);
//        for j = i+1:b_max+1
//            if min > PreComputation(j, 3)
//                imin = j;
//                min = PreComputation(j, 3);                
//            end;
//        end;
        
//        for j = 1:3
//            temp = PreComputation(imin, j);
//            PreComputation(imin, j) = PreComputation(i, j);
//            PreComputation(i, j) = temp;
//        end;        
//    end;
    
//    i = b_max + 1;
//    length_1 = 0;
//    length_max = 0;
//    while i > 0 && length_1 >= length_max
//        j = legth_k;
//        length_max = length_1;
//        while j > 0 && bitget(PreComputation(i, 3),j) == bitget(k,j)
//            j = j - 1;
//        end;
        
//        length_1 = legth_k - j;
//        i = i - 1;
//    end;
    
//    if length_1 < length_max
//        i = i + 2;
//    else
//        i = 1;
//    end;
    
//    b1 = PreComputation(i, 1);
//    a1 = legth_k - get_number_bit(PreComputation(i, 2), 2);
    
//    if a1 < 0
//        a1 = 0;
//    end;
    
//    % якщо максимальне співпадіння по бітах трапилось не на останньому
//    % елементі масиву, то розглянути елемент на 1 старший від обраного,
//    % оскільки обраний елемент b дає наближення з недостачею, а старший за
//    % нього з надлишком
//    if i < b_max+1
//        b2 = PreComputation(i+1, 1);
//        a2 = legth_k - get_number_bit(PreComputation(i+1, 2), 2);    
//        if a2 < 0
//            a2 = 0;
//        end;
//    else
//        b2 = 0;
//        a2 = 0;
//    end;
    
//    % якщо обране значення а є більшим за максимально допустиме, то
//    % встановити поточне значення а як максимально допустиме та змінити
//    % відповідним чином значення b
//    [ a1, b1 ] = Re_Compute_a_b(a1, b1, a_max, b_max, k);    
//    [ a2, b2 ] = Re_Compute_a_b(a2, b2, a_max, b_max, k);
    
//    % визначити яка з пар показників степеня дає краще наближення
//    if abs(k - 2^a1*3^b1) < abs(k - 2^a2*3^b2)
//        a = a1;
//        b = b1;
//    else
//        a = a2;
//        b = b2;
//    end;
    
//    % обране значення a дає наближення з недостачею, перевірити чи дасть
//    % краще наближення з надлишком наступне значення a
//    if a ~= a_max && abs(k - 2^(a+1)*3^b) < abs(k - 2^a*3^b)
//        a = a + 1;
//    end;
    
//end





//function [ value ] = log_dif_base (base, argument)

//    value = log(argument) / log(base);

//end

//function [ number_bit ] = get_number_bit (value, base)

//    number_bit = floor(log_dif_base(base, value));
//    if log_dif_base(base, value) > number_bit || ~number_bit
//        number_bit = number_bit + 1;
//    end;

//end

//function [ a, b ] = Re_Compute_a_b(a, b, a_max, b_max, k)

//    if a > a_max
//        temp = get_number_bit(2^(a - a_max), 3) - 1;
//        b = b + temp;
        
//        if b > b_max
//            b = b_max;
//        end;
        
//        if a_max > 0
//            temp = a_max - 1;
//        else
//            temp = 0;
//        end;
        
//        if abs(k - 2^a_max*3^b) < abs(k - 2^temp*3^(b + 1)) || b == b_max
//            a = a_max;
//        else
//            a = temp;
//            b = b + 1;
//        end;            
//    end;

//end

//#endregion
//#region 20

//function [ x2, y2, z2 ] = Point_Multiplication_Affine_Coord_20 (x1, y1, z1, a, k, p)


//    a_max = 15;
//    b_max = 17;
//    mas_k = zeros(1,3);
//%     [ mas_k ] = Convert_to_DBNS_1 (k, a_max, b_max);
//    [ mas_k ] = Convert_to_DBNS_2 (k, a_max, b_max);
    
//    % перевіримо чи правильно виконали розкладення
//    sum = 0;
//    s = size(mas_k);
//    for i = 1:s(1)
//        sum = sum + mas_k(i,1)*2^mas_k(i,2)*3^mas_k(i,3);
//    end;
    
//    if (k ~= sum)
//        disp('ALERT');
//    end

   
//    x2 = x1;
//    y2 = mas_k(1,1)*y1;
//    z2 = z1;
        
//    for i = 1:s(1)-1
//        u = mas_k(i, 2) - mas_k(i + 1, 2);
//        v = mas_k(i, 3) - mas_k(i + 1, 3);
        
//        for j = 1:u
//            [ x2, y2, z2 ] = Double_Affine_Coord (x2, y2, z2, a, p);
//        end;
        
//        for j = 1:v
//            [ x2, y2, z2 ] = Ternary_Affine_Coord (x2, y2, z2, a, p);
//        end;
        
//        [ x2, y2, z2 ] = Add_Affine_Coord (x1, mas_k(i + 1,1)*y1, z1, x2, y2, z2, a, p);
//    end;
    
    
//    for j = 1:mas_k(s(1), 2)
//        [ x2, y2, z2 ] = Double_Affine_Coord (x2, y2, z2, a, p);
//    end;
    
//    for j = 1:mas_k(s(1), 3)
//        [ x2, y2, z2 ] = Ternary_Affine_Coord (x2, y2, z2, a, p);
//    end;
    
    
//    if ~x2 && y2
//        z2 = 0;
//    else
//        z2 = 1;
//    end;
    
//end

//#endregion
//#region 21

//function [ x3, y3, z3 ] = Point_Multiplication_Affine_Coord_21 (x1, y1, z1, a, k, p)


//    mas_k = zeros(1,3);
//    [ mas_k ] = Convert_to_DBNS (k);
    
//    % перевіримо чи правильно виконали розкладення
//    sum = 0;
//    temp = 1;
//    s = size(mas_k);
//    for i = 1:s(1)
//        temp = temp * 2^mas_k(i,2)*3^mas_k(i,3);
//        sum = sum + mas_k(i,1)*temp;
//    end;
    
//    if (k ~= sum)
//        disp('ALERT');
//    end

    
//    x3 = 0;
//    y3 = 1;
//    z3 = 0;
        
//    for i = 1:s(1)
        
//        for j = 1:mas_k(i,2)
//            [ x1, y1, z1 ] = Double_Affine_Coord (x1, y1, z1, a, p);
//        end;
        
//        for j = 1:mas_k(i,3)
//            [ x1, y1, z1 ] = Ternary_Affine_Coord_1 (x1, y1, z1, a, p);
//        end;
        
//        [ x3, y3, z3 ] = Add_Affine_Coord (x1, mas_k(i,1)*y1, z1, x3, y3, z3, a, p);
        
//    end;
    
    
//    if ~x3 && y3
//        z3 = 0;
//    else
//        z3 = 1;
//    end;
    
//end





//function [ mas_k ] = Convert_to_DBNS (k)

//    i = 1;
//    value = k;
//    while value
        
//        a = 0;
//        while ~mod(value,2) && value
//            a = a + 1;
//            value = value / 2;
//        end;
        
//        b = 0;
//        while ~mod(value,3) && value
//            b = b + 1;
//            value = value / 3;
//        end;
        
        
//        mas_k(i, 2) = a;
//        mas_k(i, 3) = b;
        
//        value = value - 1;
//        if value
//            if mod(value, 6)
//                value = value + 2;
//                mas_k(i, 1) = -1;
//            else
//                mas_k(i, 1) = 1;
//            end;
//        else
//            mas_k(i, 1) = 1;
//        end;
        
//        i = i + 1;
        
//    end;
   
//end



//#endregion
//#region 22

//function [ x2, y2, z2 ] = Point_Multiplication_Affine_Coord_22 (x1, y1, z1, a, k, p)


//    mas_k = zeros(1,3);
//    [ mas_k ] = Convert_to_DBNS (k);
    
//    % перевіримо чи правильно виконали розкладення
//    sum = 1;
//    s = size(mas_k);
//    for i = s(1):-1:1
//        sum = sum * 2^mas_k(i,2)*3^mas_k(i,3) + mas_k(i,1);
//    end;

    
//    if (k ~= sum)
//        disp('ALERT');
//    end

   
//    x2 = x1;
//    y2 = y1;
//    z2 = z1;
        
//    for i = s(1):-1:2
        
//        for j = 1:mas_k(i,2)
//            [ x2, y2, z2 ] = Double_Affine_Coord (x2, y2, z2, a, p);
//        end;
        
//        for j = 1:mas_k(i,3)
//            [ x2, y2, z2 ] = Ternary_Affine_Coord_1 (x2, y2, z2, a, p);
//        end;
        
//        [ x2, y2, z2 ] = Add_Affine_Coord (x1, mas_k(i,1)*y1, z1, x2, y2, z2, a, p);
        
//    end;
    
    
//    for j = 1:mas_k(1,2)
//        [ x2, y2, z2 ] = Double_Affine_Coord (x2, y2, z2, a, p);
//    end;
    
//    for j = 1:mas_k(1,3)
//        [ x2, y2, z2 ] = Ternary_Affine_Coord (x2, y2, z2, a, p);
//    end;
    
    
//    if ~x2 && y2
//        z2 = 0;
//    else
//        z2 = 1;
//    end;
    
//end

//#endregion
