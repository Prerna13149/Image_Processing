cmake_minimum_required(VERSION 2.8)
project( IA_Assign3 )
find_package( OpenCV REQUIRED )
include_directories( ${OpenCV_INCLUDE_DIRS} )

add_executable( IA_Assign3_4 IA_Assign3_4.cpp )
target_link_libraries( IA_Assign3_4 ${OpenCV_LIBS} )