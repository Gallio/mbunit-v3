#!/bin/bash
here=$(dirname "$0")
bin="$here/bin"
src="$here/src"

cd "$src"

export CFLAGS="-DMINGW -mno-cygwin"
export CXXFLAGS="-DMINGW -mno-cygwin"

if [[ ! -e Makefile ]]
then
  ./configure \
    --disable-gen-cpp \
    --disable-gen-java \
    --enable-gen-csharp \
    --disable-gen-py \
    --disable-gen-rb \
    --disable-gen-perl \
    --disable-gen-php \
    --disable-gen-erl \
    --disable-gen-cocoa \
    --disable-gen-st \
    --disable-gen-ocaml \
    --disable-gen-hs \
    --disable-gen-xsd \
    --disable-gen-html \
    --with-csharp \
    --without-java \
    --without-erlang \
    --without-py \
    --without-perl \
    --without-ruby
else
  echo "Skipping configure script because Makefile already exists."
fi

make -C compiler/cpp clean all
cp "compiler/cpp/.libs/thrift.exe" "$bin"
#cp "/bin/cygwin1.dll" "$bin"

"$SYSTEMROOT/Microsoft.Net/Framework/v3.5/msbuild.exe" "lib\\csharp\\src\\Thrift.csproj" /p:Configuration=Release
cp "lib/csharp/src/bin/Release/Thrift.dll" "$bin"
cp "lib/csharp/src/bin/Release/Thrift.pdb" "$bin"

"$SYSTEMROOT/Microsoft.Net/Framework/v3.5/msbuild.exe" "lib\\csharp\\ThriftMSBuildTask\\ThriftMSBuildTask.csproj" /p:Configuration=Release
cp "lib/csharp/ThriftMSBuildTask/bin/Release/ThriftMSBuildTask.dll" "$bin"
cp "lib/csharp/ThriftMSBuildTask/bin/Release/ThriftMSBuildTask.pdb" "$bin"
